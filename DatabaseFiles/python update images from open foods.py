#Uploaded the products_import.txt

#This file contains an image column, not all images work. Please test each image and for any that return a 404 error add a new column at the row end to indicate the image does not exist. do this for the full file

#alter this script to add a new alternate image as a new column named image2, this image must be proven to work

#alter the file to fix broken tesco cdn links automatically

#when testing the URL I am getting access denied - can you alter the code to test the original image via a img ref html block

#ok when the original image fails the code to generate a working image is not working, please generate a replacement image for each of the failed original images by looking up the product image using the gtin13

import csv
import requests
import re

input_file = "C:\\SourceCode\\MatthewPage2026FinalYearProject\\DatabaseFiles\\Image check.txt"
output_file = "C:\\SourceCode\\MatthewPage2026FinalYearProject\\DatabaseFiles\\Image check_fixed.txt"
count=0

headers = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36",
    "Accept": "image/webp,image/apng,image/*,*/*;q=0.8",
    "Referer": "https://www.tesco.com/"
}


def image_exists(url):
    try:
        r = requests.get(url, headers=headers, stream=True, timeout=5)

        if r.status_code == 200 and "image" in r.headers.get("Content-Type", ""):
            return r.status_code

    except:
        pass

    return 999


def get_product_image(gtin):

    try:
        url = f"https://world.openfoodfacts.org/api/v0/product/{gtin}.json"

        r = requests.get(url, timeout=5)

        if r.status_code == 200:

            data = r.json()

            if data["status"] == 1:

                image = data["product"].get("image_front_url")

                if image and image_exists(image):
                    return image

    except:
        pass

    return ""


with open(input_file, "r", encoding="utf-8") as infile, open(output_file, "w", encoding="utf-8", newline="") as outfile:

    reader = csv.reader(infile, delimiter="\t")
    writer = csv.writer(outfile, delimiter="\t")

    header = next(reader)
    header.append("ImageFixed")
    writer.writerow(header)

    for row in reader:

        gtin = row[1]
        image_url = row[11]

        #check if existing image is working ok
        reply = image_exists(image_url)
        if reply ==200:
            imagestatus = "Image working"
            image2 = ""
        else:
            image2 = get_product_image(gtin)
            #check new image exists
            if image2 == "":
                imagestatus = "No replacement available"
            else:
                reply = image_exists(image2)
                if reply ==200:
                    imagestatus = "Replaced + Checked Working"
                else:
                    imagestatus = "Replaced + unclear if working"
                        

        row.append(imagestatus)
        row.append(image2)
        writer.writerow(row)
        count=count+1
        print("On row", count)
        #if count == 15:
        #    break

print("Finished fixing Tesco image URLs.")