from data import UnityAPI, Camera, Transform, Vector3, Vector2
from pythonAPI import SceneSnapshot

#----------OBJECT----------
objectPosition = Vector3(0, -1, 0)
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
imageBytes = SceneSnapshot(data, filename)
