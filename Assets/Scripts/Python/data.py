class Vector2:
    def __init__(self, x, y):
        self.x = x
        self.y = y


class Vector3:
    def __init__(self, x, y, z):
        self.x = x
        self.y = y
        self.z = z


class Transform:
    def __init__(self, position = Vector3(0, 0, 0), rotation = Vector3(0, 0, 0), scale = Vector3(1, 1, 1)):
        self.position = position
        self.rotation = rotation
        self.scale = scale


class UnityObject(Transform):
    def __init__(self, transform: Transform, path: str = None):
        super().__init__(transform.position, transform.rotation, transform.scale)
        self.path = path


class Camera(Transform):
    def __init__(self, transform: Transform, fov: float, resolution: Vector2):
        super().__init__(transform.position, transform.rotation, transform.scale)
        self.fov = fov
        self.resolution = resolution
        

class Illumination(Transform):
    def __init__(self, transform: Transform, intensity: float = 2.0):
        super().__init__(transform.position, transform.rotation, transform.scale)
        self.intensity = intensity


class Screenshot:
    def __init__(self, take_screenshot, remove_background=True, tolerance=0.7) -> None:
        self.take_screenshot = take_screenshot
        self.remove_background = remove_background
        self.tolerance = tolerance


class APIData:
    def __init__(self, unity_object: UnityObject = None, camera: Camera = None, screenshot: Screenshot = None, illumination: Illumination = None, message: str | None = None):
        # AO ADICIONAR ALGO AQUI, ADD NO UNITY TBM -APIPythonUnityJsonFormat.cs-
        self.unity_object = unity_object
        self.camera = camera
        self.screenshot = screenshot
        self.illumination = illumination
        self.message = message
        self.__update_unity_object = True if unity_object is not None else False
        self.__update_camera = True if camera is not None else False
        self.__update_screenshot = True if screenshot is not None else False
        self.__update_illumination = True if illumination is not None else False



# #-----------------TEST--------------------

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
