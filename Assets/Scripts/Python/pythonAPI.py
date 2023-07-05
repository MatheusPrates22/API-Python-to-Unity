import socket
import keyboard
import os
from data import UnityAPI, Camera, Transform, Vector3, Vector2

host = 'localhost'  # Endereço IP ou nome do host do Unity
port = 12345  # Porta do Unity

BUFFER_SIZE = 1024


udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
udp_socket.connect((host, port))

def SceneSnapshot(data: UnityAPI):
    message = data.toJson().encode('utf-8')
    # print(message)

    udp_socket.send(message)
    
    dataReceived, addr = udp_socket.recvfrom(BUFFER_SIZE)
    amountOfPackages = dataReceived.decode()
    print("Quantidade de pacotes: " + amountOfPackages)
    amountOfPackages = int(amountOfPackages)

    image_data = b""
    while True: 
        if amountOfPackages > 0:
            dataReceived, addr = udp_socket.recvfrom(BUFFER_SIZE)
            image_data += dataReceived
            amountOfPackages  = amountOfPackages - 1
        else:
            break

    print("\n-----Socket closed-----")
    udp_socket.close()
    
    
    print("\n-----Gerando imagem-----")
    with open("Assets\\Photos\\imageTest.png", "wb") as file:
        file.write(image_data)


#----------OBJECT----------
objectPosition = Vector3(-1, 0, 0)
objectRotation = Vector3(0, 0, 0)
objectScale = Vector3(2, 1, 1)
objectTransform = Transform(objectPosition, objectRotation, objectScale)

#----------CAMERA----------
cameraPosition = Vector3(0, 0, -3.17)
cameraRotation = Vector3(0, 0, 0)
cameraScale = Vector3(1, 1, 1)
cameraTransform = Transform(cameraPosition, cameraRotation, cameraScale)
camera = Camera(cameraTransform, 60, Vector2(1920, 1080))

#---------API-----------
data = UnityAPI("Pessoa", 1, objectTransform, camera)
SceneSnapshot(data)