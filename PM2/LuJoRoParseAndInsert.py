import json
import zipfile
import psycopg2
import os

#from this directory, there is a data file that has the zip file with the json files
os.chdir(r'C:\Users\lucas.dasilva\Documents\WSUSpring2021\Cpts_451\PM2')


# change the parameters (like dbname and password) for it to work on your computer
def psycopg2_connect():
    return psycopg2.connect("dbname='yelp' user='postgres' host='localhost' password='5829'")


# # put data files in data folder
# def extract_zipfiles(filepath=r'C:\Users\lucas.dasilva\Documents\WSUSpring2021\Cpts_451\PM2\data', folderpath=r'./data'):
#     with zipfile.ZipFile(filepath, 'r') as zip_ref:
#         zip_ref.extractall(folderpath)


def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")


def int2BoolStr (value):
    if value == 0:
        return 'False'
    else:
        return 'True'


def create_cmd(d, tablename):
    cmd = f"INSERT INTO {tablename} " + "(" + ", ".join(d.keys()) + ")" + " VALUES " + "(" + ", ".join("'" + str(e) + "'" for e in d.values()) + ");"
    return cmd   


def create_d(data, strings, floats, ints, bools):
    d = {}

    for name in strings:
        d[name] = cleanStr4SQL(str(data[name]))
        
    for name in floats:
        d[name] = float(cleanStr4SQL(str(data[name])))
        
    for name in ints:
        d[name] = int(cleanStr4SQL(str(data[name])))
        
    for name in bools:
        value = int(cleanStr4SQL(str(data[name])))
        d[name] = int2BoolStr(value)
    
    return d


def insert_row(conn, cur, d, tablename):
    try:
        cmd = create_cmd(d, tablename)
        cur.execute(cmd)
    except Exception as e:
        print(f"Insert to {tablename} failed!",e)
    
    conn.commit()


def insertBusinessData():
    with open(r'./data/yelp_business.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        
        #connect to database
        conn = psycopg2_connect()
        cur = conn.cursor()
        
        #features to be extracted from json
        strings = ['business_id', 'name', 'address', 'state', 'city', 'postal_code']
        floats = ['latitude', 'longitude', 'stars']
        ints = ['review_count']
        bools = ['is_open']
        
        while line:
            data = json.loads(line)
            
            #insert into Business table
            d = create_d(data, strings, floats, ints, bools)
            insert_row(conn, cur, d, tablename='business')
            
            #insert into BusinessCategories table
            categories = [cleanStr4SQL(category) for category in data["categories"].split(', ')]
            business_id = data["business_id"]
            for category in categories:
                cat_d = {'business_id':business_id, 'category':category}
                insert_row(conn, cur, d=cat_d, tablename='businesscategories')
                          
            #hours data
            for day, hours in data['hours'].items():
                #TODO: 
                # - seperate start and end hour into two sep vars
                # - change sql table
                day_d = {'business_id':business_id, 'day':day, 'hours':hours}
                insert_row(conn, cur, d=day_d, tablename='WeekHours')
            
            #done with reading current line
            count_line += 1
            line = f.readline()
            if count_line % 100 == 0:
                print(f'Finshed reading row {count_line}.')
            
        cur.close()
        conn.close()
        print('Finished with yelp_business.JSON file\n\n')


def insertUserData_1():
    with open(r'./data/yelp_user.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        
        #connect to database
        conn = psycopg2_connect()
        cur = conn.cursor()
        
        #features to be extracted from json
        strings = ['user_id', 'name', 'yelping_since']
        floats = ['average_stars']
        ints = ['cool', 'fans', 'funny', 'tipcount', 'useful']
        bools = []
        
        while line:
            data = json.loads(line)
            
            #insert into usertable table
            d = create_d(data, strings, floats, ints, bools)
            insert_row(conn, cur, d, tablename='usertable')
            
            #done with reading current line
            count_line += 1
            line = f.readline()
            if count_line % 1000 == 0:
                print(f'Finshed reading row {count_line}.')
            
        cur.close()
        conn.close()
        print('Finished with yelp_user.JSON file (Part 1/2)\n\n')


def insertUserData_2():
    with open(r'./data/yelp_user.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        
        #connect to database
        conn = psycopg2_connect()
        cur = conn.cursor()
        
        while line:
            data = json.loads(line)
            
            #insert into Friendship table
            user_id = data["user_id"]
            for friend_id in data["friends"]:
                friend_d = {'first_user_id':user_id, 'second_user_id':friend_id}
                insert_row(conn, cur, d=friend_d, tablename='Friendship')
            
            #done with reading current line
            count_line += 1
            line = f.readline()
            if count_line % 1000 == 0:
                print(f'Finshed reading row {count_line}.')
            
        cur.close()
        conn.close()
        print('Finished with yelp_user.JSON file (Part 2/2)\n\n')


def insertCheckinData():
    with open(r'./data/yelp_checkin.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        
        #connect to database
        conn = psycopg2_connect()
        cur = conn.cursor()
        
        while line:
            data = json.loads(line)
            business_id = cleanStr4SQL(data['business_id'])

            dates = data["date"].split(',')
            for date in dates:
                date_d = {'business_id':business_id, 'checkin_date':date}
                insert_row(conn, cur, d=date_d, tablename='CheckIn')

            #done with reading current line
            count_line += 1
            line = f.readline()
            if count_line % 100 == 0:
                print(f'Finshed reading row {count_line}.')
            
        cur.close()
        conn.close()
        print('Finished with yelp_checkin.JSON file\n\n')


def insertTipData():
    with open(r'./data/yelp_tip.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        
        #connect to database
        conn = psycopg2_connect()
        cur = conn.cursor()
        
        #features to be extracted from json
        strings = ['business_id', 'user_id', 'text', 'date']
        floats = []
        ints = ['likes']
        bools = []
        
        while line:
            data = json.loads(line)
            
            #insert into usertable table
            d = create_d(data, strings, floats, ints, bools)
            insert_row(conn, cur, d, tablename='Tip')
            
            #done with reading current line
            count_line += 1
            line = f.readline()
            if count_line % 1000 == 0:
                print(f'Finshed reading row {count_line}.')
            
        cur.close()
        conn.close()
        print('Finished with yelp_tip.JSON file\n\n')


if __name__ == "__main__":
    # insertBusinessData()
    # insertUserData_1()
    # insertUserData_2()
    insertCheckinData()
    insertTipData()