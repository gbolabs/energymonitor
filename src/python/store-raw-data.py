def writeTofile(data):
    rawFile = open("raw.json", "w")
    rawFile.write(data)
    rawFile.close()