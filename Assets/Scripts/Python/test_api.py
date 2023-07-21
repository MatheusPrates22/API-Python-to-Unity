from data import UnityAPI, Camera, Transform, Vector3, Vector2
from api import API

#----------OBJECT----------
objectPosition = Vector3(-1.2, -1, -1)
objectRotation = Vector3(-90, 0, 180)
objectScale = Vector3(1, 1, 1)
objectTransform = Transform(objectPosition, objectRotation, objectScale)

#----------CAMERA----------
cameraPosition = Vector3(0, 0, -3.17)
cameraRotation = Vector3(0, 0, 0)
cameraScale = Vector3(1, 1, 1)
cameraTransform = Transform(cameraPosition, cameraRotation, cameraScale)
camera = Camera(cameraTransform, 60, Vector2(1920, 1080))

#---------API-----------
data = UnityAPI("Pessoa", 1, objectTransform, camera)
filename = r"imageAPI.png"
# imageBytes = SceneSnapshot(data, filename)

my_api = API()
# my_api = API(startConnection=False)
# my_api.connect()
# my_api.send_message("Test API")
# my_api.send_update_scene(data)
my_api.scene_snapshot(data, show=True)
# my_api.take_screenshot(show=True)
