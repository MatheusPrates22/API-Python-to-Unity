import socket

host = 'localhost'  # Endereço IP ou nome do host do Unity
port = 12345  # Porta do Unity
server_address = (host, port)

# Estabelece a conexão com o servidor Unity
client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client.connect(server_address)

# Envia uma mensagem para o servidor
message = "Olá, servidor Unity!"
client.sendall(message.encode())

# Recebe a resposta do servidor
response = client.recv(4096).decode()
print("Resposta do servidor: ", response)

# Fecha a conexão
client.close()