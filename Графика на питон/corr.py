
import requests
import matplotlib.pyplot as plt

body = {
    'latitudeStart': 52.690478,
    'latitudeEnd': 60,
    'longtitudeStart': 155,
    'longtitudeEnd': 159.525422,
    'intervalDays': 800,
    'stepDays': 300
}

response = requests.post(
    'http://localhost:5255/api/EarthQuakes/AutoCorrelation', json=body).json()


for row in response:
    plt.figure()
    plt.plot([i["t"] for i in row["values"]], [i["value"] for i in row["values"]],
             label='Автокорреляционная функция')
    plt.show()
    a = input()
    if len(a) > 0:
        break
