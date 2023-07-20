import socket
import json
from os import path
from data import APIPythonUnityJsonFormat, UnityAPI, Transform
from config_manager import ConfigManager


class API():
    def __init__(self, startConnection = True):
        self.__initConfigManager()
        if startConnection:
            self.connect()

    def connect(self, host = None, port = None, protocol = None):
        if host is None:
            self.host = self.__configManager.host

        if port is None:
            self.port = self.__configManager.port

        if protocol is None:
            self.protocol = self.__configManager.protocol

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

    def isConnected(self):
        return self.socket is not None

    def sendMessage(self, message: str, save_json=False):
        print("Sending message")
        if self.__isNotConnected():
            return False
        json_data = json.dumps(APIPythonUnityJsonFormat(message=message).__dict__)
        self.__sendJson(json_data, save_json=save_json)

    def sendUpdateScene(self, data_to_update_scene: UnityAPI, save_json=False):
        print("Send update scene")
        if self.__isNotConnected():
            return False
        json_data = json.dumps(APIPythonUnityJsonFormat(data=data_to_update_scene.toJson()).__dict__)
        self.__sendJson(json_data, save_json=save_json)
        
    def takeScreenshot(self, save_json=False):
        print("Send take screenshot")
        if self.__isNotConnected():
            return False
        json_data = json.dumps(APIPythonUnityJsonFormat(take_screenshot=True))
        self.__sendJson(json_data, save_json=save_json)

    def sceneSnapshot(self, data_to_update_scene: UnityAPI, save_json=False):
        print("Send scene snapshot")
        if self.__isNotConnected():
            return False
        json_data = json.dumps(APIPythonUnityJsonFormat(data=data_to_update_scene.toJson(), take_screenshot=True).__dict__)
        self.__sendJson(json_data, save_json=save_json)
        image_bytes = self.__readBytesReceived()
        self.saveBytes(data_bytes=image_bytes)

    def removeImageBackground(self):
        ...
    
    def saveBytes(self, data_bytes: bytes, filename = r"imageAPI.png"):
        filepath = path.join("Assets", "Photos", filename)
        with open(filepath, "wb") as file:
            file.write(data_bytes)

    def __readBytesReceived(self) -> bytes:
        dataReceived, _ = self.socket.recvfrom(self.__configManager.buffer_size)
        amountOfPackages = int(dataReceived.decode())

        image_data = b""
        while True:
            if amountOfPackages > 0:
                dataReceived, addr = self.socket.recvfrom(self.__configManager.buffer_size)
                image_data += dataReceived
                amountOfPackages  = amountOfPackages - 1
            else:
                break
        return image_data

    def __changeObject(self, object: str):
        ...

    def __changeObjectTransform(self, transform: Transform):
        ...

    def __changeCameraTransform(self, transform: Transform):
        ...

    def __sendJson(self, json_data: str, save_json=False):
        if save_json:
            self.__saveJson(json_data)
        dataBytes = json_data.encode()
        self.__sendBytes(dataBytes)

    def __sendBytes(self, dataBytes: bytes):
        if self.__isNotConnected():
            return False
        self.socket.send(dataBytes)

    def __initConfigManager(self):
        self.host = None
        self.port = None
        self.protocol = None
        self.__configManager = ConfigManager()
        self.__configManager.load_config(path.join(path.dirname(__file__),'configAPI.json'))

    def __isNotConnected(self):
        if not self.isConnected():
            print("A conexão não está estabelecida.")
            return True
        return False
    
    def __saveJson(self, my_json: str, filename = 'api.json'):
        path_to_save = path.join(path.dirname(__file__), filename)
        with open(path_to_save, 'w') as arquivo_json:
            arquivo_json.write(my_json)

