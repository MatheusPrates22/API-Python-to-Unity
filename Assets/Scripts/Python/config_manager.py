import json
from enum import Enum

class ConfigManager:
    def __init__(self):
        self.host = None
        self.port = None
        self.protocol = None
        self.buffer_size = 0
        self.data_types = {}

    def load_config(self, config_file):
        with open(config_file, 'r') as f:
            config_data = json.load(f)
            self.host = config_data['host']
            self.port = config_data['port']
            self.protocol = config_data['protocol']
            self.buffer_size = config_data['buffer_size']