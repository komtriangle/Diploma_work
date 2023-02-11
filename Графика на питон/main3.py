import requests
import matplotlib.pyplot as plt
import numpy as np
import os
import glob
import stat
import math


# построение графиков зависимости
# корреляционной размерности от размерности вложения
def drawCorrelationDimension(response):
    clearFolder(
        'C:\Учеба\НИР\Код\Diploma.WebApi\Графика на питон\correlationDimension')
    for interval in response["timeIntervalCorrelationDimensions"]:
        x = [i["m"] for i in interval["correlationDimensions"]]
        y = [i["value"] for i in interval["correlationDimensions"]]
        fig, ax = plt.subplots(nrows=1, ncols=1)  # create figure & 1 axis
        ax.plot(x, y)
        # save the figure to file
        fig.savefig(
            f'C:\Учеба\НИР\Код\Diploma.WebApi\Графика на питон\correlationDimension\{interval["minDimension"]}---{interval["start"][:10]}--{interval["end"][:10]}')  #
        plt.close(fig)

# построение графиков корреляционных интегралов


def drawCorelationIntegrals(response):
    clearFolder(
        'C:\Учеба\НИР\Код\Diploma.WebApi\Графика на питон\correlationIntegrals')
    for interval in response["timeIntervalCorrelationDimensions"]:
        fig, ax = plt.subplots(nrows=1, ncols=1)
        ax.plot(sigmas_ln, interval["attractor_tmp"])
        y = [interval["k_tmp"]*x + interval["b_tmp"] for x in sigmas_ln]
        ax.plot(sigmas_ln, y)
        fig.savefig(
            f'C:\Учеба\НИР\Код\Diploma.WebApi\Графика на питон\correlationIntegrals\{interval["start"][:10]}--{interval["end"][:10]}')  #
        plt.close(fig)


def clearFolder(path):
    files = glob.glob(path)
    for f in files:
        try:
            os.chmod(f, stat.S_IWRITE)
            os.remove(f)
        except:
            print("error")

# body = {
#     'latitudeStart': 52.690478,
#     'latitudeEnd': 60,
#     'longtitudeStart': 155,
#     'longtitudeEnd': 159.525422,
#     'intervalDays': 800,
#     'stepDays': 300
# }


body = {
    'latitudeStart': 41.55792,
    'latitudeEnd': 44.30813,
    'longtitudeStart':  41.06689,
    'longtitudeEnd': 48.7793,
    'intervalDays': 700,
    'stepDays': 200
}


sigmas = [3, 2.9, 2.8, 2.7, 2.6, 2.5, 2.4, 2.3, 2.2, 2.1, 2.0, 1.9,
          1.8, 1.7, 1.6, 1.5, 1.4, 1.3, 1.2, 1.1]

sigmas_ln = [i for i in sigmas]  # [math.log(i) for i in sigmas]

response = requests.post(
    'http://localhost:5255/api/EarthQuakes/AnalyzeEarthQuakes', json=body).json()
dimensions = [time_interval["minDimension"]
              for time_interval in response["timeIntervalCorrelationDimensions"]]
magnitudes = [row["magnitude"] for row in response["timeSeriesMaxMagnitudes"]]

drawCorrelationDimension(response)
drawCorelationIntegrals(response)


print(np.corrcoef(dimensions, magnitudes)[0, 1])

fig, ax = plt.subplots()
ax.plot(list(range(len(dimensions))), dimensions,
        label='Корреляционная размерность')
ax.plot(list(range(len(magnitudes))), magnitudes, label='Магнитуда')
plt.legend()
plt.show()
