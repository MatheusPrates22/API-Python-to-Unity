import socket
from data import UnityAPI

host = 'localhost'  # Endereço IP ou nome do host do Unity
port = 12345  # Porta do Unity

BUFFER_SIZE = 1024 #Tamanho do pacote a ser recebido do Unity


udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
udp_socket.connect((host, port))

def SceneSnapshot(data: UnityAPI, filename = r"Assets\\Photos\\imageAPI.png"):
    #manda os dados para o unity
    message = data.toJson().encode('utf-8')
    udp_socket.send(message)
    
    #recebe a quantidade de pacotes (a imagem é dividida em pacotes de até BUFFER_SIZE bytes)
    dataReceived, addr = udp_socket.recvfrom(BUFFER_SIZE)
    amountOfPackages = dataReceived.decode()
    amountOfPackages = int(amountOfPackages)

    #le todos os pacotes que recebeu do unity para formar a imagem
    image_data = b""
    while True: 
        if amountOfPackages > 0:
            dataReceived, addr = udp_socket.recvfrom(BUFFER_SIZE)
            image_data += dataReceived
            amountOfPackages  = amountOfPackages - 1
        else:
            break

    #fecha a conexão com unity
    udp_socket.close()
    
    #Salva a imagem
    with open(filename, "wb") as file:
        file.write(image_data)

    return image_data
