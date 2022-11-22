# More details can be found in TechToTinker.blogspot.com 
# George Bantique | tech.to.tinker@gmail.com

from mfrc522 import MFRC522
import network
from machine import Pin
from machine import SPI
import urequests
import ubinascii
import ujson


MacAddress = ''
ScannedId = ''
CompanyName = 'Howest'
inventory = {"rdr":""}
oldinventory = inventory

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
        if 


do_connect()

spi.init()
rdr = MFRC522(spi=spi, gpioRst=4, gpioCs=5)

inventroyScan()
print("Place card")

while True:
    
    (stat, tag_type) = rdr.request(rdr.REQIDL)
    if stat == rdr.OK:
        (stat, raw_uid) = rdr.anticoll()
        if stat == rdr.OK:
            card_id = "0x%02x%02x%02x%02x" % (raw_uid[0], raw_uid[1], raw_uid[2], raw_uid[3])
            print(card_id)
            
            if card_id not in inventory:
                inventory["rdr"] = card_id
                post_data = ujson.dumps({ 'MacAddress': MacAddress, 'ScannedId': str(card_id), 'CompanyName': str(CompanyName)})
                response = urequests.post("", headers = {'content-type': 'application/json'}, data = post_data).json()
     
     else:
         inventory["rdr"] = ""
         #beginnen met factureren na 5 keer niet gescand te zijn.
         
            
            
            

