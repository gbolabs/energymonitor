import json
import datetime

def parse_powermeter_data(rsPayload):
    # Find index of 1.8.1
    idx181 = rsPayload.index("1.8.1")
    idx182 = rsPayload.index("1.8.2")
    idxIL1 = rsPayload.index("31.7.0")
    idxIL2 = rsPayload.index("51.7.0")
    idxIL3 = rsPayload.index("71.7.0")
    

    # Missing extracting the value from the whole read text
    # TODO: Here


# Expects an array with the read values in sequence order;
# [0] -> accumulated consumed energy high-tarif [KWh]
# [1] -> accumulated consumed energy low-tarif [kWh]
# [2] -> currently delivered current L1 [A]
# [3] -> currently delivered current L2 [A]
# [4] -> currently delivered current L3 [A]
def store_powermeter_data(readData):
    try:
        # read file
        with open('data.json', 'r') as last_read:
            data_json=last_read.read()

        # parse file
        data = json.loads(data_json)
    except:
        print('Unable to read_data history assumes none existed.')
        defaults = '{"sampling":"'+str(datetime.now())+'", "consumedHighTarif":0,"consumedLowTarif":0,"liveCurrentL1":0,"liveCurrentL2":0,"liveCurrentL3":0}'
        data = json.loads(defaults)

    # Stored consumed energy
    data['sampling']= str(datetime.now())
    data['consumedHighTarif'] = extractEnergy(readData[0])
    data['consumedLowTarif'] = extractEnergy(readData[1])

    data['liveCurrentL1'] = extractLiveCurrent(readData[2])
    data['liveCurrentL2'] = extractLiveCurrent(readData[3])
    data['liveCurrentL3'] = extractLiveCurrent(readData[4])

    try:
        jsonString = json.dumps(data)
        jsonFile = open("data.json", "w")
        jsonFile.write(jsonString)
        jsonFile.close()
    except:
        print('Unable to write_data history. will not be able to compute delta consumption.')


# 1.8.1(025139.058*kWh)
def extractEnergy(consumedEnergy):
    return consumedEnergy[7,16]


# 31.7.0(000.71)
def extractLiveCurrent(liveCurrent):
    return liveCurrent[7,12]