from flask import Flask, request, jsonify
import json

app = Flask(__name__)

DATA_FILE = 'earthquake_data.json'
VALID_API_KEY = 'your-secret-api-key'

@app.route('/api/send-data', methods=['POST'])
def receive_data():
    api_key = request.headers.get('X-API-Key')
    
    if api_key != VALID_API_KEY:
        return jsonify({"status": "error", "message": "Invalid API Key"}), 403

    data = request.json
    with open(DATA_FILE, 'a') as f:
        json.dump(data, f)
        f.write('\n')

    return jsonify({"status": "success"}), 200

if __name__ == '__main__':
    app.run(host='127.0.0.1', port=8266)
