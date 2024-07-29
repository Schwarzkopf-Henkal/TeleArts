import pyrealsense2 as rs
import socket
import cv2
import numpy as np
import struct
 
# 配置 RealSense 相机
pipeline = rs.pipeline()
config = rs.config()
config.enable_stream(rs.stream.color, 640, 480, rs.format.bgr8, 30)
 
# 开始捕获视频
pipeline.start(config)
 
# 配置 UDP 服务器
UDP_IP = "192.168.137.34"  # 目标计算机的 IP 地址
UDP_PORT = 5005
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
 
while True:
    # 获取帧
    frames = pipeline.wait_for_frames()
    color_frame = frames.get_color_frame()
 
    # 将帧转换为 OpenCV 图像
    color_image = np.asanyarray(color_frame.get_data())
 
    # 显示图像
    cv2.imshow('RealSense server', color_image)
 
    # 将图像转换为字符串并发送到目标计算机
    # data = color_image.tostring()
 
    # 将图像转换为JPEG格式
    _, jpeg_image = cv2.imencode('.jpg', color_image)
    data = jpeg_image.tobytes()
    # 【定义文件头、数据】
    fread = struct.pack('i', len(data))
    # print(len(data))
    # 【发送文件头、数据】
    sock.sendto(fread, (UDP_IP, UDP_PORT))
    # 每次发送x字节，计算所需发送次数
    pack_size = 65507
    send_times = len(data) // pack_size + 1
    # print(send_times)
    for count in range(send_times):
        if count < send_times - 1:
            sock.sendto(data[pack_size * count:pack_size * (count + 1)], (UDP_IP, UDP_PORT))
        else:
            sock.sendto(data[pack_size * count:],(UDP_IP, UDP_PORT))
    # sock.sendto(data, (UDP_IP, UDP_PORT))
 
    # 按下 ESC 键退出循环
    if cv2.waitKey(1) == 27:
        break
 
# 停止捕获视频并关闭窗口
pipeline.stop()
cv2.destroyAllWindows()