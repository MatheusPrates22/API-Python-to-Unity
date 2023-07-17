import socket
from os import path
from data import UnityAPI

host = 'localhost'  # Endereço IP ou nome do host do Unity
port = 12345  # Porta do Unity
server_address = (host, port)

BUFFER_SIZE = 1024 #Tamanho do pacote a ser recebido do Unity


# udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
udp_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
udp_socket.connect(server_address)

def SceneSnapshot(data: UnityAPI, filename = r"imageAPI.png"):
# def SceneSnapshot(data: UnityAPI, filename = r"Assets\\Photos\\imageAPI.png"):
    #manda os dados para o unity
    message = data.toJson().encode('utf-8')
    udp_socket.send(message)
    
    #recebe a quantidade de pacotes (a imagem é dividida em pacotes de até BUFFER_SIZE bytes)
    dataReceived, addr = udp_socket.recvfrom(BUFFER_SIZE)
    amountOfPackages = dataReceived.decode()
    # print("Packages: " + amountOfPackages)
    amountOfPackages = int(amountOfPackages)

    #le todos os pacotes que recebeu do unity para formar a imagem
    image_data = b""
    while True: 
        if amountOfPackages > 0:
            dataReceived, addr = udp_socket.recvfrom(BUFFER_SIZE)
            image_data += dataReceived
            amountOfPackages  = amountOfPackages - 1
            # if (amountOfPackages % 20 == 0 or amountOfPackages < 5):
                # print("Packages left: " + str(amountOfPackages))
        else:
            break

    #fecha a conexão com unity
    udp_socket.close()
    
    # print("Gerando imagem")
    #Salva a imagem
    filepath = path.join("Assets", "Photos", filename)
    with open(filepath, "wb") as file:
        file.write(image_data)

    return image_data
