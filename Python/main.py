import os
import cv2
import numpy as np
import json

# Change path to find image and mask
filename = "DSC00944.png"
path = os.getcwd()
split_path = path.split('\\')
parent_path = ""
for i in range(len(split_path) - 1):
    parent_path += split_path[i] + "\\"

resources_path = parent_path + "Assets\\Resources\\"
os.chdir(resources_path)

# Image
image_path = resources_path + "Images\\" + filename
im = cv2.imread(image_path)
im = cv2.resize(im, None, fx=1/10, fy=1/10)

# Show Image
# image_window = "Image"
# cv2.namedWindow(image_window)
# cv2.moveWindow(image_window, 20, 20)
# cv2.imshow(image_window, im)
# cv2.waitKey()

# Mask
mask_path = resources_path + "Masks\\" + filename
mask = cv2.imread(mask_path)
mask = cv2.resize(mask, None, fx=1/10, fy=1/10)

# Show mask
# mask_window = "Mask"
# cv2.namedWindow(mask_window)
# cv2.imshow(mask_window, mask)
# cv2.waitKey()

# Reshape mask into [?? x 3] shape to find all colors in image
reshaped_mask = mask.reshape(-1, mask.shape[-1])
all_colors = np.unique((np.unique(reshaped_mask, axis=0) != 0) * 128, axis=0)

# Stores all calculated contours
raw_contours = []
all_contours_canvas =  np.zeros(mask.shape)

for color in all_colors:
    if (color != [0, 0, 0]).any() and (color != [128, 128, 128]).any():
        new_mask = ((mask == color).all(axis=2).astype(int)*255).astype(np.uint8)
        new_mask = (np.expand_dims(new_mask, axis=2) * np.array([1, 1, 1])).astype(np.uint8)

        # Show mask
        # mask_window = "Mask"
        # cv2.namedWindow(mask_window)
        # cv2.imshow(mask_window, new_mask)
        # cv2.waitKey()

        # Show image through the mask
        # mask_window = "Result"
        # cv2.namedWindow(mask_window)
        # cv2.imshow(mask_window, cv2.bitwise_and(new_mask, im))
        # cv2.waitKey()

        # Calculates contours in mask
        contour_mask = cv2.cvtColor(new_mask, cv2.COLOR_BGR2GRAY)
        contours, hierarchy = cv2.findContours(contour_mask, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
        raw_contours.append(contours)

        # Show contours in image
        contour_window = "Contour"
        #contour_canvas = np.zeros(new_mask.shape)
        #cv2.drawContours(contour_canvas, contours, -1, (0, 255, 0), 3)
        cv2.drawContours(all_contours_canvas, contours, -1, (0, 255, 0), 3)
        #cv2.imshow(contour_window, contour_canvas)
        #cv2.waitKey()

#
cv2.imshow(contour_window, all_contours_canvas)
cv2.waitKey()

# Separates each contour
all_contours = {"contours": []}
for contour in raw_contours:
    for i in range(len(contour)):
        # Calculates contour length for each limb
        all_contours["contours"].append(
            {"contourPoints":
                 [{"x": int(contour[i][j][0][0]),
                   "y": int(contour[i][j][0][1])} for j in range(len(contour[i]))],
             "contourLength": cv2.arcLength(contour[i], True)})

        # Show each contour on black canvas
        #contour_canvas = np.zeros(new_mask.shape)
        #cv2.drawContours(contour_canvas, contour, i, (0, 255, 0), 3)
        #contour_window = "Contour"
        #cv2.imshow(contour_window, contour_canvas)
        #cv2.waitKey()


# Find overall contour
full_mask = ((mask == [0, 0, 0]).all(axis=2).astype(int)*255).astype(np.uint8)
full_mask = (np.expand_dims(full_mask, axis=2) * np.array([1])).astype(np.uint8)
full_mask = cv2.bitwise_not(full_mask)
contours, hierarchy = cv2.findContours(full_mask, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

overall_contour = {"contour": []}
for i in range(len(overall_contour)):
    # Calculates contour length for each limb
    overall_contour["contour"].append(
        {"contourPoints":
             [{"x": int(contour[i][j][0][0]),
               "y": int(contour[i][j][0][1])} for j in range(len(contour[i]))],
         "contourLength": cv2.arcLength(contour[i], True)})

# Show each contour on black canvas
contour_canvas = np.zeros(full_mask.shape)
cv2.drawContours(contour_canvas, contours,  -1, 255, 3)
contour_window = "Contour"
cv2.imshow(contour_window, contour_canvas)
cv2.waitKey()


# print(all_contours)

# Toggle between the first (to write the JSON in Unity) or the second (to write the JSON in the Python folder)
# json_filename = resources_path + "Jsons\\contours.json"
# json_filename = path + "contours.json"

# with open(json_filename, "w") as json_file:
#     json_file.write(json.dumps(all_contours))

# Global contour
#new_mask = ((mask == [0, 0, 0]).all(axis=2).astype(int)*255).astype(np.uint8)
#new_mask = (np.expand_dims(new_mask, axis=2) * np.array([1, 1, 1])).astype(np.uint8)

#contour_mask = cv2.cvtColor(new_mask, cv2.COLOR_BGR2GRAY)
#contours, hierarchy = cv2.findContours(contour_mask, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

# Show mask
#contour_canvas = np.zeros(new_mask.shape)
#cv2.drawContours(contour_canvas, contours, i, (0, 255, 0), 3)
#contour_window = "Contour"
#cv2.imshow(contour_window, contour_canvas)
#cv2.waitKey()


# Toggle between the first (to write the JSON in Unity) or the second (to write the JSON in the Python folder)
# json_filename = resources_path + "Jsons\\global_contours.json"
# json_filename = path + "contours.json"

# with open(json_filename, "w") as json_file:
#     json_file.write(json.dumps(all_contours))
