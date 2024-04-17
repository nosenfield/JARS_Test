import glob
import shutil
import time
import random

def continuous_event_stream_simulator(events_directory, broadcast_directory, min_buffer_size, wait_time):
    while(True):
        if(len(glob.glob(broadcast_directory + "/*")) <= min_buffer_size):
            # randomly choose file from events directory
            cached_files = glob.glob(events_directory + "/*")
            random_file = random.choice(cached_files)
            # copy file to broadcast directory
            shutil.copy(random_file, broadcast_directory)
        else:
            print("waiting for broadcast directory to be ingested")
            time.sleep(wait_time)

BROADCAST_DIRECTORY = "./unity_input"
EVENTS_DIRECTORY = "./off_limits/events"
WAIT_TIME = 4
MIN_BUFFER_SIZE = 1

continuous_event_stream_simulator(EVENTS_DIRECTORY, BROADCAST_DIRECTORY, MIN_BUFFER_SIZE, WAIT_TIME)