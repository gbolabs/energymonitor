import timeit
import time

startTime = timeit.default_timer()

# Do some long processing
time.sleep(1)

endtime = timeit.default_timer()
delta = endtime-startTime
print("Time taken: "+str(delta))