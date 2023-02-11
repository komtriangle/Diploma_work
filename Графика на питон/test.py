
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt

%matplotlib inline

def easy(earthquake_filename):
    
    # выгрузка данных
    df= pd.read_excel(earthquake_filename)
    grid_df = grid(df)
    
    # отрисовка сетки с точками из df
    points = [df['latitude'], df['longitude']]
    #plt.scatter(grid_df[:, 0], grid_df[:, 1], alpha = 0)
    plt.scatter(points[0], points[1], color = 'orange', label='Earthquake')
    plt.xlabel('latitude')
    plt.ylabel('longitude')
    plt.legend()
    plt.grid()
    
    df['zone'] = list(map(lambda x, y : str(zone([x, y], grid_df)), df['latitude'], 
                                                                    df['longitude']))
    df['m.y'] = df['month'].astype(str) + '.' + df['year'].astype(str)
    table_df = pd.pivot_table(df, values='mag', index='m.y', columns='zone', aggfunc=np.max, 
                                                             fill_value=df['mag'].mean())
    plt.savefig('map.png')
    return table_df

def grid(df):
    grid = []
    for x in range(np.int(df['latitude'].min()), np.int(df['latitude'].max()) + 3, 2):
        for y in range(np.int(df['longitude'].min()), np.int(df['longitude'].max()) + 3, 2):
            grid.append([x, y])
    grid = np.array(grid)
    return grid

def zone(point, grid):
    x = np.unique(grid[:, 0])
    y = np.unique(grid[:, 1])
    zones = []
    for i in range(1, len(x)):
        for j in range(1, len(y)):
            if point[0] >= x[i-1] and point[0] < x[i] and point[1] >= y[j-1] and point[1] < y[j]:
                return [x[i-1], y[j-1]]

def paint(table, sigma):
    dict_, korr_int, sigm_log = func3(table.values, sigma)
    plt.plot([i for i in dict_.keys()], [i for i in dict_.values()], label='sigma={}'.format(sigma), marker='o')    

    print(dict_)
    plt.title('D(r)')
    plt.xlabel('r')
    plt.ylabel('D')
    return dict_, korr_int, sigm_log

import sys
import math
def func3(array, sigmas):
    rd = {}
    korr_sigma = {}
    sigm_log = [0] * len(sigmas)
    korr_int = [0] * len(sigmas)
    for r in range(2, 16):
        distance = [0] * array.shape[1]
        #print(r)
        for i1 in range(1, array.shape[1]-r):
            temp = 0
            for i2 in range(i1-1, i1 + r-1):
                #sys.stdout.write(str(i2) + ' ')
                for t in range(0, array.shape[0]):
                    temp += (array[t][i2] - array[t][i2-1]) ** 2
            distance[i1-1] = np.sqrt(temp)
            #print(distance[i1-1])        
        k = 0
        dist = {}
        for i in range(0, len(distance)-1):
            for j in range(i+1, len(distance)):
                #print(k)
                #dist[k] = print(distance[i] - distance[j])
                dist[k] = distance[i] - distance[j]
                k += 1
        D = 0
        for i in range (0, len(sigmas)):
            sigm_log = [0] * len(sigmas)
            korr_int = [0] * len(sigmas)
            hevi = 0
            for j in range(0, len(dist)):
                if sigmas[i] - dist[j] >= 0:
                    hevi += 1
            #hevi -= 1
            #print(hevi)
            korr_int[i] = np.log(hevi / (array.shape[1])** 2)
            D += -(korr_int[i]) / np.abs(np.log(sigmas[i]))
            #D += np.abs(korr_int[i]) / np.abs(np.log(sigmas[i]))
            sigm_log[i] = (np.log(sigmas[i]))
            #sys.stdout.write('korr_int: ' + str(korr_int[i]) + ' sigma: ' + str(np.log(sigmas[i])) + '\n')
        rd[r] = D / len(sigmas)
        korr_sigma[r] = [korr_int, sigm_log]
    return rd, korr_sigma
