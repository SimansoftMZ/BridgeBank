from setuptools import setup, find_packages

setup(
    name="bridgebank-sdk",
    version="1.0.0",
    description="BridgeBank API client for Python - Bank reconciliation, transaction classification, and payment generation",
    author="Simansoft",
    url="https://github.com/SimansoftMZ/BridgeBank",
    packages=find_packages(),
    python_requires=">=3.9",
    install_requires=[
        "microsoft-kiota-bundle==1.9.8",
    ],
)
