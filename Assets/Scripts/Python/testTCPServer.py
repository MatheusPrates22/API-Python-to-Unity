import socket
from os import path
from config_manager import ConfigManager

configManager = ConfigManager()
configManager.load_config(path.join(path.dirname(__file__),'configAPI.json'))

server_address = (configManager.host, configManager.port)

# udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
tcp_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
tcp_socket.connect(server_address)

msg = "Olá, from Python22!"
msg_bytes = msg.encode()
tcp_socket.send(msg_bytes)

tcp_socket.close()


