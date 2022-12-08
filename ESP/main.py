# More details can be found in TechToTinker.blogspot.com 
# George Bantique | tech.to.tinker@gmail.com

from mfrc522 import MFRC522
import network
from machine import Pin
from machine import SPI
import urequests
import ubinascii
import ujson
import json
import time
import gc


MacAddress = ''
ScannedId = ''
CompanyName = 'Howest'
inventory = {"rdr":""}
oldinventory = inventory
scanapiURL = "https://thecollective.azurewebsites.net/api/v1/scans"
inout = False
sendEnd1 = True

teller1 = 0

spi = SPI(2, baudrate=2500000, polarity=0, phase=0)
# Using Hardware SPI pins:
#     sck=18   # yellow
#     mosi=23  # orange
#     miso=19  # blue
#     rst=4    # white
#     cs=5     # green, DS
# *************************
# To use SoftSPI,
# from machine import SOftSPI
# spi = SoftSPI(baudrate=100000, polarity=0, phase=0, sck=sck, mosi=mosi, miso=miso)
def do_connect():
    
    global MacAddress
    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    wlan_mac = wlan.config('mac')
    MacAddress = ubinascii.hexlify(wlan_mac).decode()
    if not wlan.isconnected():
        print('connecting to network...')
        wlan.connect('Howest-IoT', 'LZe5buMyZUcDpLY')
        while not wlan.isconnected():
            pass
    print('network config:', wlan.ifconfig())

def inventoryScan():
    global inventory
    global card_id
    (stat, tag_type) = rdr.request(rdr.REQIDL)
    if stat == rdr.OK:
        (stat, raw_uid) = rdr.anticoll()
        if stat == rdr.OK:
            card_id = "0x%02x%02x%02x%02x" % (raw_uid[0], raw_uid[1], raw_uid[2], raw_uid[3])
            inventory["rdr"] = card_id
    

def compareInventory():
    global inventory
    global oldinventory
    if inventory == oldinventory:
        pass
    else:
        oldinventory = inventory


do_connect()

spi.init()
rdr = MFRC522(spi=spi, gpioRst=4, gpioCs=5)

inventoryScan()
print("Place card")

while True:
    gc.collect()
    response = None
    try:
        (stat, tag_type) = rdr.request(rdr.REQIDL)
        if stat == rdr.OK:
            (stat, raw_uid) = rdr.anticoll()
            if stat == rdr.OK:
                card_id = "0x%02x%02x%02x%02x" % (raw_uid[0], raw_uid[1], raw_uid[2], raw_uid[3])
                #print("cardid: " + card_id)
                print(inventory.values())
                teller1 = 0
                
                if card_id not in inventory.values():
                    teller1 = 0
                    inventory["rdr"] = card_id
                    post_data = ujson.dumps({ 'MacAddress': MacAddress, 'ScannedId': str(card_id), 'CompanyName': str(CompanyName), 'InOut': True})
                    print("send " + post_data)
                    response = urequests.post("https://thecollective.azurewebsites.net/api/v1/scans", headers = {'content-type': 'application/json'}, data = post_data).json()
                    print(response)
                    sendEnd1 = True
                    print("end factuur")
                    #stoppen met factureren

        elif stat != rdr.OK:
            (stat, tag_type) = rdr.request(rdr.REQIDL)
            if stat != rdr.OK:
                #beginnen met factureren na 5 keer niet gescand te zijn.
                teller1 = teller1 + 1
                if teller1 >= 5 and sendEnd1 and inventory["rdr"] != "":
                    #beginnen met factureren
                    inventory["rdr"] = ""
                    print("begin factuur")
                    sendEnd1 = False
                    post_data = ujson.dumps({ 'MacAddress': MacAddress, 'ScannedId': str(card_id), 'CompanyName': str(CompanyName), 'InOut': False})
                    response = urequests.post("https://thecollective.azurewebsites.net/api/v1/scans", headers = {'content-type': 'application/json'}, data = post_data)
                    response.close()
                else:
                    print("klaar")
                    print(teller1)
                    if teller1 >= 6:
                        teller1 = 0
         
            
        time.sleep(1)
    except Exception as e: # Here it catches any error.
        if isinstance(e, OSError) and response: # If the error is an OSError the socket has to be closed.
            response.close()
            inventory["rdr"] = ""
        value = {"error": e}
        print(value)
    gc.collect()
            

