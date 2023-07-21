import socket
import json
from os import path
from data import APIPythonUnityJsonFormat, UnityAPI, Transform
from config_manager import ConfigManager
from rembg import remove as rembg_remove
from PIL import Image
from io import BytesIO
from typing import Union

class API():
    def __init__(self, start_connection = True, image_filepath=None, image_filename=None):
        self.__image = None
        self.__init_config_manager()
        self.set_image_filepath(image_filepath)
        self.set_image_filename(image_filename)
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
            return True
        except socket.error as err:
            print(f"Erro ao conectar: {err}")
            return False

    def disconnect(self):
        if self.socket is not None:
            self.socket.close()
            self.socket = None

    def is_connected(self):
        return self.socket is not None

    def send_message(self, message: str, save_json=False):
        print("Sending message")
        if self.__is_not_connected():
            return False
        json_data = json.dumps(APIPythonUnityJsonFormat(message=message).__dict__)
        self.__send_json(json_data, save_json=save_json)

    def send_update_scene(self, data_to_update_scene: UnityAPI, save_json=False):
        print("Send update scene")
        if self.__is_not_connected():
            return False
        json_data = json.dumps(APIPythonUnityJsonFormat(data=data_to_update_scene.toJson()).__dict__)
        self.__send_json(json_data, save_json=save_json)
        
    def take_screenshot(self, save_json=False, save_image: bool=False, save_path: str=None, show: bool=False) -> Image.Image:
        print("Send take screenshot")
        if self.__is_not_connected():
            return False
        #Converte para json e manda pro unity
        json_data = json.dumps(APIPythonUnityJsonFormat(take_screenshot=True).__dict__)
        self.__send_json(json_data, save_json=save_json)
        #le a imagem recebida
        image = self.__read_image_bytes_received()
        if save_image:
            self.save_image(image, save_path)
        if show:
            image.show()
        return image

    def remove_image_background(self, image_path: str | Image.Image, save: bool=False, save_path: str=None, show: bool=False) -> Image.Image:
        try:
            if isinstance(image_path, str):
                image = Image.open(image_path)
                self.set_image(image)
            else:
                image = self.set_image(image_path)
            image_without_background = self.set_image_without_background(rembg_remove(image))
            if save:
                self.save_image(image_without_background, save_path)
            if show:
                image_without_background.show()
            return image_without_background
        except Exception as e:
            print("Erro ao remover o background da imagem: " + e)

    def take_screenshot_and_remove_background(self, save_json: bool=False, save_image: bool=False, save_path: str=None, show: bool=False) -> Image.Image:
        image = self.set_image(self.take_screenshot(save_json=save_json))
        return self.set_image_without_background(self.remove_image_background(image_path=image, save=save_image, save_path=save_path, show=show))

    def scene_snapshot(self, data_to_update_scene: UnityAPI, save_json=False, save_image: bool=False, save_path: str=None, show: bool=False):
        print("Send scene snapshot")
        if self.__is_not_connected():
            return False
        #Converte para json e manda pro unity
        json_data = json.dumps(APIPythonUnityJsonFormat(data=data_to_update_scene.toJson(), take_screenshot=True).__dict__)
        self.__send_json(json_data, save_json=save_json)
        #le a imagem recebida
        image = self.__read_image_bytes_received()
        #remove background da imagem recebida
        return self.remove_image_background(image, save=save_image, save_path=save_path, show=show)

    def save_image(self, image: Image.Image, save_path: str=None):
        if save_path is None:
            save_path = self.image_abs_filepath()
        image.save(save_path)


    def image(self) -> Image.Image:
        return self.__image
    
    def set_image(self, image: Image.Image) -> Image.Image:
        if isinstance(image, Image.Image):
            self.__image = image
            return self.__image

    def image_without_background(self) -> Image.Image:
        return self.__image_without__background

    def set_image_without_background(self, image: Image.Image) -> Image.Image:
        if isinstance(image, Image.Image):
            self.__image_without__background = image
            return self.__image_without__background

    def image_filepath(self) -> str:
        if self.__image_filepath is None:
            self.set_image_filepath()
        return self.__image_filepath

    def set_image_filepath(self, filepath: str=None) -> str:
        if filepath is None:
            filepath = path.join("Assets", "Photos")
        self.__image_filepath = filepath
        return self.__image_filepath

    def image_filename(self) -> str:
        if self.__image_filename is None:
            self.set_image_filename()
        return self.__image_filename
    
    def set_image_filename(self, filename: str=r"imageAPI.png") -> str:
        self.__image_filename = filename
        return self.__image_filename

    def image_abs_filepath(self) -> str:
        if self.__image_abs_filepath is None:
            self.set_image_abs_filepath()
        return self.__image_abs_filepath

    def set_image_abs_filepath(self, abs_filepath: str=None) -> str:
        if abs_filepath is None:
            abs_filepath = path.join(self.image_filepath(), self.image_filename())
        self.__image_abs_filepath = abs_filepath
        return self.__image_abs_filepath

    def __save_image_bytes(self, data_bytes: bytes, filepath: str=None):
        if filepath is None:
            filepath = self.image_abs_filepath()
        with open(filepath, "wb") as file:
            file.write(data_bytes)

    def __read_image_bytes_received(self) -> Image.Image:
        image_bytes = self.__read_bytes_received()
        #converte para uma Imagem
        image_bytes_io = BytesIO(image_bytes)
        return self.set_image(Image.open(image_bytes_io))

    def __read_bytes_received(self) -> bytes:
        data_received, _ = self.socket.recvfrom(self.__config_manager.buffer_size)
        amount_of_packages = int(data_received.decode())

        image_data = b""
        while True:
            if amount_of_packages > 0:
                data_received, addr = self.socket.recvfrom(self.__config_manager.buffer_size)
                image_data += data_received
                amount_of_packages  = amount_of_packages - 1
            else:
                break
        return image_data

    def ___read_image(self, imagePath):
        ...

    def __change_object(self, object: str):
        ...

    def __change_object_transform(self, transform: Transform):
        ...

    def __change_camera_transform(self, transform: Transform):
        ...

    def __send_json(self, json_data: str, save_json=False):
        if save_json:
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
        self.__config_manager.load_config(path.join(path.dirname(__file__),'configAPI.json'))

    def __is_not_connected(self):
        if not self.is_connected():
            print("A conexão não está estabelecida.")
            return True
        return False
    
    def __save_json(self, my_json: str, filename = 'api.json'):
        path_to_save = path.join(path.dirname(__file__), filename)
        with open(path_to_save, 'w') as arquivo_json:
            arquivo_json.write(my_json)

