def writeTofile(data):
    rawFile = open("/home/pi/raw.data", "w")
    rawFile.write(data)
    rawFile.close()