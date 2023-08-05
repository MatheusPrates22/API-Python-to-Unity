import socket
import json
import os
from data import APIData, UnityObject, Camera, Screenshot, Illumination
from config_manager import ConfigManager
from rembg import remove as rembg_remove
from PIL import Image
from io import BytesIO
from typing import Union


class API():
    __SAVE_SEND_JSON = True

    def __init__(self, start_connection = True, image_filepath=None, image_filename=None):
        self.__image = None
        self.__init_config_manager()
        self.__init_paths(image_filepath, image_filename)
        if start_connection:
            self.connect()

    def connect(self, host = None, port = None, protocol = None):
        if host is None:
            self.host = self.__config_manager.host

        if port is None:
            self.port = self.__config_manager.port

        if protocol is None:
            self.protocol = self.__config_manager.protocol

        try:
            if self.protocol == "TCP":
                self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            else:
                self.socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            self.socket.connect((self.host, self.port))
            self.__is_connect = True
        except socket.error as err:
            print(f"Failed to connect: {err}")
            self.__is_connect = False

    def disconnect(self):
        if self.socket is not None:
            self.socket.close()
            self.socket = None

    def send_message(self, message: str):
        if self.__is_not_connected():
            return False
        print("Sending message")
        json_data = APIData(message=message)
        self.__send_json(self.__to_json(json_data))

    def send_update_object(self, unity_object: UnityObject):
        self.send_update_scene(unity_object=unity_object)

    def send_update_camera(self, camera: Camera):
        self.send_update_scene(camera=camera)

    def send_update_Illumination(self, illumination: Illumination):
        self.send_update_scene(illumination=illumination)

    def send_update_scene(self, unity_object: UnityObject = None, camera: Camera = None, illumination: Illumination = None):
        if self.__is_not_connected():
            return False
        print("Send update scene")
        json_data = APIData(unity_object=unity_object, camera=camera, illumination=illumination)
        self.__send_json(self.__to_json(json_data))
        
    def take_screenshot(self, screenshot: Screenshot, save_image=False, save_filename: str | None = None, show=False) -> Image.Image:
        if self.__is_not_connected():
            return False
        print("Send take screenshot")
        json_data = APIData(screenshot=screenshot)
        self.__send_json(self.__to_json(json_data))
        #le a imagem recebida
        return self.__read_image_bytes_received(save_image=save_image, save_filename=save_filename, show=show)

    def scene_snapshot(self, data_to_update_scene: APIData, save_image=False, save_filename: str | None = None, show=False):
        """
        Performs changes in the Unity scene, takes a snapshot of the scene, and removes the background.

        Parameters:
        data_to_update_scene (UnityAPI): Instance of the UnityAPI class containing the data to update the scene in Unity.
        save_image (bool, optional): Defines whether the received image should be saved in PNG format (default is False).
        save_filename (str or None, optional): File name to save the image. If set to None (default), the filename will be automatically set to "imageAPI.png".
        show (bool, optional): Defines whether the image should be displayed (default is False).

        Returns:
        bytes: The bytes of the received image.
        """
        if self.__is_not_connected():
            return False
        print("Send scene snapshot")
        
        # Converte para json e manda pro unity
        self.__send_json(self.__to_json(data_to_update_scene))
        
        # Le a imagem recebida
        return self.__read_image_bytes_received(save_image=save_image, save_filename=save_filename, show=show)

    def save_image(self, image: Image.Image, save_filename: str = None) -> None:
        if save_filename is not None and isinstance(save_filename, str):
            self.image_filename = save_filename
        image.save(self.image_abs_filepath)
        # self.image = Image.open(save_filename)

    def load_image(self, imagePath: str) -> None:
        self.image = Image.open(imagePath)

    def show_image(self, title: str | None = None) -> None:
        if title is None:
            title = self.image_filename
        self.image.show(title)

    def remove_image_background_using_python(self, image_path: str | Image.Image, save=False, save_path: str| None = None, show=False) -> Image.Image:
        try:
            if isinstance(image_path, str):
                image_path = Image.open(image_path)
            self.image = rembg_remove(image_path)
            if save:
                self.save_image(self.image, save_path)
            if show:
                self.show_image(self.image_filename)
            return self.image
        except Exception as e:
            print("Erro ao remover o background da imagem: " + e)

    ##### ----------------- PROPERTY'S -----------------
    @property
    def is_connected(self):
        return self.__is_connect

    @is_connected.setter
    def is_connected(self, value: bool):
        if isinstance(value, bool):
            self.__is_connect = value
    
    @property
    def image(self) -> Image.Image:
        return self.__image
    
    @image.setter
    def image(self, image: Image.Image) -> Image.Image:
        if isinstance(image, Image.Image):
            self.__image = image
            return self.__image

    @property
    def image_filepath(self) -> str:
        if self.__image_filepath is None:
            self.image_filepath = None
        return self.__image_filepath

    @image_filepath.setter
    def image_filepath(self, filepath: str=None) -> None:
        # Caminho relativo padrão
        if filepath is None:
            filepath = os.path.join("Assets", "Photos")

        # Converte para caminho absoluto
        filepath = os.path.abspath(filepath)

        # Cria o diretório se não existir
        if not os.path.exists(filepath):
            print(f"Directory created: {filepath}")
            os.makedirs(filepath)

        # Atualiza o atributo
        self.__image_filepath = filepath
        self.image_abs_filepath = None

    @property
    def image_filename(self) -> str:
        if self.__image_filename is None:
            self.image_filename = None
        return self.__image_filename
    
    @image_filename.setter
    def image_filename(self, filename: str=None) -> None:
        if filename is None:
            filename = r"imageAPI.png"
        self.__image_filename = filename
        self.image_abs_filepath = None

    @property
    def image_abs_filepath(self) -> str:
        if self.__image_abs_filepath is None:
            self.image_abs_filepath = None
        return self.__image_abs_filepath

    @image_abs_filepath.setter
    def image_abs_filepath(self, abs_filepath: str=None) -> None:
        if abs_filepath is None:
            abs_filepath = os.path.join(self.image_filepath, self.image_filename)
        self.__image_abs_filepath = abs_filepath

    def __save_image_bytes(self, data_bytes: bytes, filepath: str=None):
        if filepath is None:
            filepath = self.image_abs_filepath()
        with open(filepath, "wb") as file:
            file.write(data_bytes)

    ##### ----------------- END PROPERTY'S -----------------

    ##### ----------------- PRIVATE METHODS -----------------
    def __read_image_bytes_received(self, save_image: bool=False, save_filename: str=None, show: bool=False) -> Image.Image:
        image_bytes = self.__read_bytes_received()

        # Converte para uma Imagem
        image_bytes_io = BytesIO(image_bytes)
        self.image = Image.open(image_bytes_io)

        # Salva e mostra se necessário
        if save_image:
            self.save_image(self.image, save_filename)
        if show:
            self.show_image(self.image_filename)
        return self.image

    def __read_bytes_received(self) -> bytes:
        data_received, _ = self.socket.recvfrom(self.__config_manager.buffer_size)
        amount_of_packages = int(data_received.decode())

        image_data = b""
        while True:
            if amount_of_packages > 0:
                data_received, _ = self.socket.recvfrom(self.__config_manager.buffer_size)
                image_data += data_received
                amount_of_packages  = amount_of_packages - 1
            else:
                break
        return image_data

    def __to_json(self, data: APIData) -> str:
        return json.dumps(data, default=lambda o: o.__dict__, 
            sort_keys=False, indent=4)

    def __send_json(self, json_data: str):
        if self.__SAVE_SEND_JSON:
            self.__save_json(json_data)
        data_bytes = json_data.encode()
        self.__send_bytes(data_bytes)

    def __send_bytes(self, data_bytes: bytes):
        if self.__is_not_connected():
            return False
        self.socket.send(data_bytes)

    def __init_config_manager(self):
        self.host = None
        self.port = None
        self.protocol = None
        self.__config_manager = ConfigManager()
        self.__config_manager.load_config(os.path.join(os.path.dirname(__file__),'configAPI.json'))

    def __init_paths(self, image_filepath, image_filename):
        self.__image_filepath = None
        self.__image_filename = None
        self.image_filepath = image_filepath
        self.image_filename = image_filename
        self.image_abs_filepath = None

    def __is_not_connected(self):
        if not self.is_connected:
            print("Connection is not established.")
            return True
        return False
    
    def __save_json(self, my_json: str, filename = 'api.json'):
        path_to_save = os.path.join(os.path.dirname(__file__), filename)
        with open(path_to_save, 'w') as arquivo_json:
            arquivo_json.write(my_json)


def setIlluminationByHour(hour: float):
    angle_per_hour = 15
    angle_at_midnight = -90

    angulo = (hour % 24) * angle_per_hour + angle_at_midnight

    return angulo

resultado = setIlluminationByHour(-14)
print(resultado) 
resultado = setIlluminationByHour(60)
print(resultado) 
resultado = setIlluminationByHour(14+24)
print(resultado) 
