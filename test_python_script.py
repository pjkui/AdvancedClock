#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
测试Python脚本 - 用于测试闹钟的Python脚本执行功能
"""

import sys
import datetime

def main():
    print("=" * 50)
    print("闹钟触发 - Python脚本执行测试")
    print("=" * 50)
    print(f"当前时间: {datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print(f"Python版本: {sys.version}")
    print("脚本执行成功！")
    print("=" * 50)
    
    return 0

if __name__ == "__main__":
    sys.exit(main())
