
# Importing Required Modules
from rembg import remove
from PIL import Image
  
# Store path of the image in the variable input_path
input_path =  r'E:\Programação\Unity Projects\API Python\Assets\Photos\CameraScreenshot2.png'
  
# Store path of the output image in the variable output_path
output_path = r'E:\Programação\Unity Projects\API Python\Assets\Photos\CameraScreenshotrbmbg.png'
  
# Processing the image
input = Image.open(input_path)
  
# Removing the background from the given Image
output = remove(input)
  
#Saving the image in the given path
output.save(output_path)