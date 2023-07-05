import json


class Vector2:
    def __init__(self, x, y):
        self.x = x
        self.y = y

class Vector3():
    def __init__(self, x, y, z):
        self.x = x
        self.y = y
        self.z = z

class Transform:
    def __init__(self, position = Vector3(0, 0, 0), rotation = Vector3(0, 0, 0), scale = Vector3(1, 1, 1)):
        self.__position = position
        self.__rotation = rotation
        self.__scale = scale

    def setPosition(self, position: Vector3):
        self.__position = position

    def position(self) -> Vector3:
        return self.__position

    def setRotation(self, rotation: Vector3):
        self.__rotation = rotation

    def rotation(self) -> Vector3:
        return self.__rotation
    
    def setScale(self, scale: Vector3):
        self.__scale = scale

    def scale(self) -> Vector3:
        return self.__scale


class Camera(Transform):
    def __init__(self, transform: Transform, fov: float, resolution: Vector2):
        super().__init__(transform.position(), transform.rotation(), transform.scale())
        # self.__transform = transform
        self.__fov = fov
        self.__resolution = resolution

    # def setTransform(self, transform: Transform):
    #     self.__transform = transform

    # def transform(self):
    #     return self.__transform

    def setFov(self, fov: float):
        self.__fov = fov

    def setResolution(self, resolution: Vector2):
        self.__resolution = resolution


class UnityAPI():
    def __init__(self, name: str, photoNumber: int, objectTransform: Transform, camera: Camera):
        self.__name = name
        self.__photoNumber = photoNumber
        self.__objectTransform = objectTransform
        self.__camera = camera

    def setObjectTransform(self, position: Vector3, rotation: Vector3, scale: Vector3):
        self.__objectTransform.setPosition(position)
        self.__objectTransform.setRotation(rotation)
        self.__objectTransform.setScale(scale)

    def setCameraTransform(self, position: Vector3, rotation: Vector3, scale: Vector3):
        self.__camera.setPosition(position)
        self.__camera.setRotation(rotation)
        self.__camera.setScale(scale)

    def setCameraPosition(self, position: Vector3):
        self.__camera.setPosition(position)

    def setCameraRotation(self, rotation: Vector3):
        self.__camera.setRotation(rotation)

    def toJson(self):
        return json.dumps(self, default=lambda o: o.__dict__, 
            sort_keys=False, indent=4)
    

# #----------OBJECT----------
# objectPosition = Vector3(0, 1, 2)
# objectRotation = Vector3(0, 2, 0)
# objectScale = Vector3(1, 3, 1)
# objectTransform = Transform(objectPosition, objectRotation, objectScale)
# # print(objectTransform.toJson())

# #----------CAMERA----------
# cameraPosition = Vector3(1, 0, -3.17)
# cameraRotation = Vector3(0, 0, 0)
# cameraScale = Vector3(1, 1, 1)
# cameraTransform = Transform(cameraPosition, cameraRotation, cameraScale)
# camera = Camera(cameraTransform, 60, Vector2(1920, 1080))

# #---------API-----------
# data = UnityAPI("Pessoa", 1, objectTransform, camera)
# # print(camera.toJson())

# import os

# filename = "teste.json"
# script_dir = os.path.dirname(os.path.abspath(__file__))
# file_path = os.path.join(script_dir, filename)

# with open(file_path, 'w') as file:
#     json_data = data.toJson()
#     file.write(json_data)
