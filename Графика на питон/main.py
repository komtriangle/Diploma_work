import requests
import matplotlib.pyplot as plt
import numpy as np


# plt.figure()
# plt.plot(list(range(len(dimensions))), dimensions)
# plt.show()
# for time_interval in response["timeIntervalCorrelationDimensions"]:
#     plt.figure()
#     plt.plot(list(range(len(time_interval["minDimension"]))), time_interval["minDimension"])
#     plt.show()


# plt.plot(list(range(len(magnitudes))), magnitudes)
# plt.show()

# def func(step, interval):
#     body ={
#     'latitudeStart': 52.690478,
#     'latitudeEnd': 60,
#     'longtitudeStart': 155,
#     'longtitudeEnd': 159.525422,
#     'intervalDays': interval,
#     'stepDays': step
#     }

def func(step, interval):
    body = {
        'latitudeStart': 41.55792,
        'latitudeEnd': 44.30813,
        'longtitudeStart':  41.06689,
        'longtitudeEnd': 48.7793,
        'intervalDays': interval,
        'stepDays': step
    }

    sigmas = [3, 2.9, 2.8, 2.7, 2.6, 2.5, 2.4, 2.3, 2.2, 2.1, 2.0, 1.9,
              1.8, 1.7, 1.6, 1.5, 1.4, 1.3, 1.2, 1.1]

    response = requests.post(
        'http://localhost:5255/api/EarthQuakes/AnalyzeEarthQuakes', json=body).json()
    dimensions = [time_interval["minDimension"]
                  for time_interval in response["timeIntervalCorrelationDimensions"]]
    magnitudes = [row["magnitude"]
                  for row in response["timeSeriesMaxMagnitudes"]]

    return np.corrcoef(dimensions, magnitudes)[0, 1]


max_coef = 0.0

for i in range(10, 600, 10):
    for j in range(300, 1500, 50):
        tmp_coef = abs(func(i, j))

        if tmp_coef > max_coef:
            max_coef = tmp_coef
            print(f'new coef: {max_coef}, step: {i}, interval: {j}')
