# detect.py
import sys
import os

def detect(image_path):
    if not os.path.exists(image_path):
        print("Image not found")
        return

    # Dummy logic: say "Fake" if filename contains "fake", otherwise "Real"
    if "fake" in image_path.lower():
        print("Fake")
    else:
        print("Real")

if __name__ == "__main__":
    detect(sys.argv[1])
