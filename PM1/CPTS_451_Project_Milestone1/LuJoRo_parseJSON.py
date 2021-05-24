import json
import zipfile
# import os
# os.chdir(r'C:\Users\josep\python_files\CptS_451\project\milestone1')


# put data files in data folder
def extract_zipfiles(filepath=r'./data/yelp_CptS451_2020.zip', folderpath=r'./data'):
    with zipfile.ZipFile(filepath, 'r') as zip_ref:
        zip_ref.extractall(folderpath)

def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")

def flatten_json(y): 
    d = {} 

    def flatten(x, name =''): 
        if type(x) is dict: 
            for attribute in x: 
                flatten(x[attribute], name + attribute + '_') 
                  
        elif type(x) is list: 
            for i, attribute in enumerate(x):
                flatten(attribute, name + str(i) + '_')

        else:
            d[name[:-1]] = x

    flatten(y)
    return d

def parseBusinessData():
    # read the JSON file
    with open(r'./data/yelp_business.JSON','r') as f:  
        outfile =  open(r'./data/business.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)
            outfile.write(cleanStr4SQL(data['business_id'])+'\t') #business id
            outfile.write(cleanStr4SQL(data['name'])+'\t') #name
            outfile.write(cleanStr4SQL(data['address'])+'\t') #full_address
            outfile.write(cleanStr4SQL(data['state'])+'\t') #state
            outfile.write(cleanStr4SQL(data['city'])+'\t') #city
            outfile.write(cleanStr4SQL(data['postal_code']) + '\t')  #zipcode
            outfile.write(str(data['latitude'])+'\t') #latitude
            outfile.write(str(data['longitude'])+'\t') #longitude
            outfile.write(str(data['stars'])+'\t') #stars
            outfile.write(str(data['review_count'])+'\t') #reviewcount
            outfile.write(str(data['is_open'])+'\t') #openstatus

            categories = data["categories"].split(', ')
            outfile.write(str(categories)+'\t')  #category list
            
            # TO-DO : write your own code to process attributes
            attributes = json.dumps(flatten_json(data['attributes']))
            outfile.write(attributes +'\t')
            
            # TO-DO : write your own code to process hours data
            week_hours = [hours for day, hours in data['hours'].items()]
            outfile.write(str(week_hours) + '\t') 
            
            # newline
            outfile.write('\n');

            line = f.readline()
            count_line += 1
    print('Number of observations parsed:', count_line)
    outfile.close()


def parseUserData():
    # TO-DO : write code to parse yelp_user.JSON
    with open(r'./data/yelp_user.JSON','r') as f:  
        outfile =  open(r'./data/user.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)
            outfile.write(cleanStr4SQL(data['name'])+'\t')
            outfile.write(cleanStr4SQL(data['user_id'])+'\t')
            
            outfile.write(str(data['average_stars'])+'\t')
            outfile.write(str(data['cool'])+'\t')
            outfile.write(str(data['fans'])+'\t') 
            outfile.write(str(data['funny'])+'\t') 
            outfile.write(str(data['tipcount'])+'\t')
            outfile.write(str(data['useful'])+'\t')
            outfile.write(str(data['yelping_since'])+'\t')

            friends = data["friends"]
            outfile.write(str(friends)+'\t')

            # newline
            outfile.write('\n');

            line = f.readline()
            count_line += 1
    print('Number of observations parsed:', count_line)
    outfile.close()


def parseCheckinData():
    # TO-DO : write code to parse yelp_user.JSON
    with open(r'./data/yelp_checkin.JSON','r') as f:  
        outfile =  open(r'./data/checkin.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)
            outfile.write(cleanStr4SQL(data['business_id'])+'\t')

            dates = data["date"].split(', ')
            outfile.write(str(dates)+'\t')

            # newline
            outfile.write('\n');

            line = f.readline()
            count_line += 1
    print('Number of observations parsed:', count_line)
    outfile.close()


def parseTipData():
    # TO-DO : write code to parse yelp_user.JSON
    with open(r'./data/yelp_tip.JSON','r') as f:  
        outfile =  open(r'./data/tip.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)
            outfile.write(cleanStr4SQL(data['business_id'])+'\t')
            outfile.write(cleanStr4SQL(data['text'])+'\t')
            outfile.write(cleanStr4SQL(data['user_id'])+'\t')
            
            outfile.write(str(data['date'])+'\t')
            outfile.write(str(data['likes'])+'\t')

            # newline
            outfile.write('\n');

            line = f.readline()
            count_line += 1
    print('Number of observations parsed:', count_line)
    outfile.close()

if __name__ == "__main__":
    parseBusinessData()
    parseUserData()
    parseCheckinData()
    parseTipData()
