from data import UnityAPI, Camera, Transform, Vector3, Vector2
from pythonAPI import SceneSnapshot

#----------OBJECT----------
objectPosition = Vector3(0.5, 0, 0)
objectRotation = Vector3(0, 0, 30)
objectScale = Vector3(2, 1, 1)
objectTransform = Transform(objectPosition, objectRotation, objectScale)

#----------CAMERA----------
cameraPosition = Vector3(0, 0, -3.17)
cameraRotation = Vector3(0, 0, 0)
cameraScale = Vector3(1, 1, 1)
cameraTransform = Transform(cameraPosition, cameraRotation, cameraScale)
camera = Camera(cameraTransform, 60, Vector2(1920, 1080))

#---------API-----------
data = UnityAPI("cars/chevrolet/0a2ae2eea2cf4073a843ee7a381c74af.glb", 1, objectTransform, camera)
filename = r"Assets\\Photos\\imageAPI.png"
imageBytes = SceneSnapshot(data, filename)
