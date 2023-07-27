from data import APIData, Camera, Transform, Vector3, Vector2, Screenshot, Illumination, UnityObject
from api import API

#----------OBJECT----------
object_position = Vector3(1.2, -1, -1)
object_rotation = Vector3(-90, 0, 180)
object_scale = Vector3(1, 1, 1)
object_transform = Transform(object_position, object_rotation, object_scale)
unity_object = UnityObject(object_transform)

#----------CAMERA----------
camera_position = Vector3(0, 0, -3.17)
camera_rotation = Vector3(0, 0, 0)
camera_scale = Vector3(1, 1, 1)
camera_transform = Transform(camera_position, camera_rotation, camera_scale)
camera = Camera(camera_transform, 60, Vector2(1920, 1080))

#----------SCREENSHOT----------
take_screenshot = True
remove_background = True
tolerance = 0.7
screenshot = Screenshot(take_screenshot, remove_background, tolerance)

#----------ILLUMINATION----------
illumination_rotation = Vector3(50, -60, 0)
illumination_transform = Transform(rotation=illumination_rotation)
illumination_intensity = 2.0
illumination = Illumination(illumination_transform, illumination_intensity)

#---------API-----------
data = APIData(unity_object, camera, screenshot, illumination)
filename = r"imageAPI.png"

my_api = API()
# my_api = API(start_connection=False)
# my_api.connect()

# my_api.scene_snapshot(data, show=True)

# my_api.send_message("Test API")
# my_api.send_update_scene(unity_object=unity_object, camera=None, illumination=illumination)
# my_api.image_filepath = r"E:\Programação\Unity Projects\API Python\Assets\Photos2"
# my_api.scene_snapshot(data, show=False, tolerance=0.6, save_image=True, save_filename='test_image.png')
# image = my_api.scene_snapshot(data, show=True, tolerance=0.7, save_image=True, save_filename='test_image2.png')
# image.show()

# my_api.scene_snapshot(data, show=False, tolerance=0.7, save_image=True, save_path=r'test_image_123.png')
# my_api.scene_snapshot(data, show=True, tolerance=0.3)
# image = my_api.take_screenshot(show=True)

# my_api.remove_image_background_using_python(image, show=True)
# my_api.take_screenshot_and_remove_background(show=True)








# my_api.scene_snapshot(data, show=True, tolerance=0.6, save_path=r"E:\Programação\Unity Projects\API Python\Assets\Photos\tolerance6.png")
# print(my_api.image_filepath)
# print(my_api.image_filename)

# MODIFICAR PARAMETROS DA ILUMINAÇÃO
# MODIFICAR OBJETO
# salvar posição e poder fazer load
# ajustar função para alterar iluminação