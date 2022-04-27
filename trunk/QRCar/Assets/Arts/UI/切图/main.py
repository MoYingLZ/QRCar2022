# This is a sample Python script.

# Press Shift+F10 to execute it or replace it with your code.
# Press Double Shift to search everywhere for classes, files, tool windows, actions, and settings.

import os
import operator
import sys
sys.setrecursionlimit(3000)

def print_hi(name):
    # Use a breakpoint in the code line below to debug your script.
    print(f'Hi, {name}')  # Press Ctrl+F8 to toggle the breakpoint.


def iterator(root_dir):
    cwd_dir = os.getcwd()
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if operator.eq(cwd_dir, root_dir) == 0:
                continue
            icon_path = os.path.join(root, file)
            print(f'file name is {file}')
            if file.startswith('.'):
                continue
            if file.endswith('.py'):
                continue

            file_name = icon_path.replace(cwd_dir, '')
            file_name = file_name.lower()
            file_name = eval(repr(file_name).replace('/','_'))
            file_name = file_name.replace('icon', 'ic')
            file_name = file_name.replace('image', 'img')
            file_name = file_name[1:]
            dest_path = os.path.join(cwd_dir, file_name)
            print(f'icon_path {icon_path}')
            print(f'dest_path {dest_path}')
            in_file = open(icon_path, 'rb')
            in_filed = in_file.read()

            out_file = open(dest_path, 'wb')
            out_file.write(in_filed)
            out_file.close()

        for dr in dirs:
            iterator(dr)


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    print_hi('PyCharm')

    icon_dir = os.getcwd()
    # icon_dir = 'D:\\Temp\\Icon'
    print(f"当前工作目录: {icon_dir}")
    iterator(icon_dir)

# See PyCharm help at https://www.jetbrains.com/help/pycharm/
