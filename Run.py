import os
import subprocess

# Modify the working directory to the folder containing the script
os.chdir(os.path.dirname(os.path.abspath(__file__)))

# Build the project
subprocess.run(["dotnet", "build"], check=True)

# Run the project
os.chdir("Projects/TerrainGenerator")
subprocess.run(["dotnet", "run"])