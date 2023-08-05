import os
from data import APIData, Camera, Transform, Vector3, Vector2, Screenshot, Illumination, UnityObject
from api import API

#----------OBJECT----------
object_position = Vector3(0, -2, 0)
object_rotation = Vector3(-90, 30, 180)
object_scale = Vector3(1.35, 1.35, 1.35)
object_transform = Transform(object_position, object_rotation, object_scale)
asset_file = "cars/acura/3d5308e5db384d5cb69464f4f46c0ea6.glb"
# asset_file = "cars/alfa romeo/1c60072517e8419a908962e3dfb8737d.glb"
# asset_file = "people/6f576f5cdf4442b18fa4810b421faaf6.glb"
asset_path = os.path.join("Assets", "Prefabs", "GLB", asset_file)
unity_object = UnityObject(object_transform, asset_path=asset_path)

#----------CAMERA----------
camera_position = Vector3(0, 0, -10)
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
illumination_intensity = 3.0
illumination_color = Vector3(0, 1, 1)
illumination = Illumination(illumination_transform, illumination_intensity, color=illumination_color)

#---------API-----------
apiData = APIData(unity_object, camera, screenshot, illumination)

# my_api = API()

# Just send a message to unity
# my_api.send_message("Test API")

# Just update the object
# my_api.send_update_object(unity_object)

# Just update the camera
# my_api.send_update_camera(camera)

# Just update the illumination
# my_api.send_update_Illumination(illumination)

# Update both
# my_api.send_update_scene(unity_object=unity_object, camera=camera, illumination=illumination)

# Just take the screenshot
# my_api.take_screenshot(screenshot, show=True)

filename = 'horario 15h.png'

# Just take the screenshot and save it
# my_api.take_screenshot(screenshot, show=False, save_image=True, save_filename=filename)

# Update scene and take the screenshot
# my_api.scene_snapshot(apiData, show=True)

# my_api.scene_snapshot(apiData, show=False, save_image=True, save_filename=filename)

# my_api.image_filepath = r"E:\Programação\Unity Projects\API Python\Assets\Photos2"
# image = my_api.scene_snapshot(apiData, save_image=True, save_filename=filename)
# image.show()


# Set different camera positions
# camera_rotation = Vector3(90, 0, 0)
# camera_scale = Vector3(1, 1, 1)

# index = 1
# for i in range(-20, 21, 10):
#     for j in range(-20, 21, 10):
#         # print("i: " + str(i) + " j: " + str(j))
#         camera_position = Vector3(i, 50, j)
#         camera_transform = Transform(camera_position, camera_rotation, camera_scale)
#         camera = Camera(camera_transform, 60, Vector2(1920, 1080))

#         filename = f'{str(index).zfill(2)} simulação de drone {i} {j}.png'
#         index += 1
#         my_api.send_update_camera(camera)
#         my_api.take_screenshot(screenshot, show=False, save_image=True, save_filename=filename)



# dimensões de um carro padrão -> C: 3.8 a 4.4  L: 1.6 a 1.85  A: 1.4 a 2.1 -> (X, Y, Z) = (1.28, 1.1, 3.19)
# dimensões do carro na cena   -> C: 3.19       L: 1.28        A: 1.1 -> (X, Y, Z) = (1.28, 1.1, 3.19)
# mudando o scale e mantendo próximo da proporção => scale 1.35


#Testar:
# não colocar chão nenhum, apenas o carro na cena
# colocar a câmera a 50m de altura olhando para o objeto
# Movimentar a câmera no céu, simulando voo de drone (mudança apenas eixo XZ)
# Rotacionar o carro
# Mudar a iluminação simulando diferentes horários

# Novas funções:
# Baseado em data, horário, latitude e longitude, definir a iluminação
# Controlar nível de nuvens no céu (sem nuvens, pouco nublado, muito nublado)

# Bugs
# Ao importar o asset/glb, alguns materiais não estão sendo atualizados

