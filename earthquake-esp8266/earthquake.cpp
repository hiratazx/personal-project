#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <MPU6050.h>

const char* ssid = "hiratazx";
const char* password = "bro-why-you-wanna-see-my-wifi-password";
const char* serverName = "http://earthquake.itzkaguya.my.id/api/send-data";
const char* apiKey = "your-secret-api-key";

MPU6050 mpu;

const int16_t threshold = 5000;

void setup() {
  Serial.begin(115200);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("Connected to WiFi");

  mpu.initialize();
}

void loop() {
  if (WiFi.status() == WL_CONNECTED) {
    int16_t ax, ay, az;
    mpu.getAcceleration(&ax, &ay, &az);

    if (abs(ax) > threshold || abs(ay) > threshold || abs(az) > threshold) {
      Serial.println("Earthquake detected, sending data...");

      WiFiClient client;
      HTTPClient http;

      http.begin(client, serverName);
      http.addHeader("Content-Type", "application/json");
      http.addHeader("X-API-Key", apiKey);

      String jsonData = "{\"ax\":" + String(ax) + ",\"ay\":" + String(ay) + ",\"az\":" + String(az) + "}";
      int httpResponseCode = http.POST(jsonData);

      if (httpResponseCode > 0) {
        String response = http.getString();
        Serial.println("Server Response: " + response);
      } else {
        Serial.print("Error sending POST: ");
        Serial.println(httpResponseCode);
      }

      http.end();
    } else {
      Serial.println("No earthquake detected.");
    }

    delay(1000);
  }
}
