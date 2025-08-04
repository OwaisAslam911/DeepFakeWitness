import sys
import cv2
import torch
import torch.nn as nn
import torchvision.transforms as transforms
from PIL import Image
import numpy as np
from efficientnet_pytorch import EfficientNet

# Load pre-trained model
model = EfficientNet.from_pretrained('efficientnet-b0')
# Add a linear layer to reduce 1000 classes to 1 for binary classification
classifier = nn.Linear(1000, 1)
model = nn.Sequential(model, classifier)
model.eval()

# Define image transformations
transform = transforms.Compose([
    transforms.Resize((224, 224)),
    transforms.ToTensor(),
    transforms.Normalize(mean=[0.485, 0.456, 0.406], std=[0.229, 0.224, 0.225])
])

def detect_deepfake(image_path):
    try:
        # Load and preprocess image
        image = cv2.imread(image_path)
        if image is None:
            return "Error: Could not load image"
        image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        image = Image.fromarray(image)
        image = transform(image).unsqueeze(0)  # Add batch dimension

        # Run inference
        with torch.no_grad():
            output = model(image)  # Shape: [1, 1]
            prediction = torch.sigmoid(output).item()  # Scalar probability

        # Threshold: >0.5 is Deepfake, <=0.5 is Real
        return "Deepfaked" if prediction > 0.5 else "Real"

    except Exception as e:
        return f"Error: {str(e)}"

if __name__ == "__main__":
    image_path = sys.argv[1]
    print(detect_deepfake(image_path))