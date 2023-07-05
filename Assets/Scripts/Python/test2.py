import socket
from data import UnityAPI, Camera, Transform, Vector3, Vector2

host = 'localhost'  # Endereço IP ou nome do host do Unity
port = 12345  # Porta do Unity
# port = 25001  # Porta do Unity


def SceneSnapshot(data: UnityAPI):
    # def SceneSnapshot(nameOrID: str, transform: Transform, iluminacao, cameraPosition: Transform) -> object: #png

    if port == 12345:
        message = data.toJson().encode('utf-8')
        # print(message)
    
        udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        udp_socket.sendto(message, (host, port))

        # Recebimento da resposta da Unity
        data, addr = udp_socket.recvfrom(1024)
        response = data.decode()

        # Exibição da resposta
        print("Resposta recebida: " + response)
        udp_socket.close()
    elif port == 25001:
        # SOCK_STREAM means TCP socket
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        try:
            print("Tentando conexão!")
            # Connect to the server and send the data
            sock.connect((host, port))
            sock.sendall(data.toJson().encode("utf-8"))
            response = sock.recv(1024).decode("utf-8")
            print (response)

        finally:
            sock.close()


#----------OBJECT----------
objectPosition = Vector3(0, 0, 0)
objectRotation = Vector3(0, 0, 0)
objectScale = Vector3(2, 1, 1)
objectTransform = Transform(objectPosition, objectRotation, objectScale)
# print(objectTransform.toJson())
# SceneSnapshot(objectTransform.toJson())

#----------CAMERA----------
cameraPosition = Vector3(0, 0, -3.17)
cameraRotation = Vector3(0, 0, 0)
cameraScale = Vector3(1, 1, 1)
cameraTransform = Transform(cameraPosition, cameraRotation, cameraScale)
camera = Camera(cameraTransform, 60, Vector2(1920, 1080))

#---------API-----------
data = UnityAPI("Pessoa", 1, objectTransform, camera)
SceneSnapshot(data)



