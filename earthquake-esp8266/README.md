## Earthquake Alert using ESP8266 ##

## 1. Beggining ##
Welp, just start here. Or if you already have the parts, skip it

## 1.1 Parts ##
What you'll need for this project, is

1. ESP8266 (NodeMCU)
2. MPU6050 (I'm using this for temporary. Maybe I'll change it soon)
3. Power supply, like USB or DIY battery?
4. Arduino IDE installed, of course

## 1.2 Connection Scheme ##
Connect the MPU6050 to ESP8266
   - VCC to 3.5V ESP8266
   - GND to GND ESP8266
   - SCL to D1 (GPIO 5)
   - SDA to D2 (GPIO 4)

And the others, just adjust it, according to your extra parts

## 2. Code ##

## 2.1 Prerequisites ##
Make sure your ESP8266 is connected, and Arduino IDE is installed
  - Add ESP8266 board into Arduino IDE
  - Check Google for more info

Install Library MPU6050
  - 'Tools > Manage Libraries' and search for MPU6050, and then install

## 2.2 The Code ##
What? The code? Just copy 'earthquake.cpp' and paste it on your IDE

## 3. Testing ##
Test it if it's works
Anyway. the README is not yet completed
