import os
import bpy

CONVERT_DIR = "/Users/pampas/workspace/unity/AgaQ/AgaQ Builder/Assets/Models/AgaQ"

def file_iter(path, extension):
	for dirpath, dirnames, filenames in os.walk(path):
		for filename in filenames:
			ext = os.path.splitext(filename)[1]
			if ext.lower().endswith(extension):
				yield os.path.join(dirpath, filename)

def reset_blend():
    bpy.ops.object.select_all(action='DESELECT')
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete()
    


def rescale():
	bpy.ops.object.editmode_toggle()
	bpy.ops.transform.resize(value=(0.025, 0.025, 0.025))
	bpy.ops.object.editmode_toggle()
    
def set_origin():
	bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_MASS', center='MEDIAN')

def convert_recursive(base_path):
	for filepath_src in file_iter(base_path, ".stl"):
		filepath_dst = os.path.splitext(filepath_src)[0] + ".blend"

		print("Converting %r -> %r" % (filepath_src, filepath_dst))

		reset_blend()
		bpy.ops.import_mesh.stl(filepath=filepath_src) 
		bpy.ops.object.select_all(action='SELECT')
		rescale()
		set_origin()
		bpy.ops.wm.save_as_mainfile(filepath=filepath_dst)


if __name__ == "__main__":
	convert_recursive(CONVERT_DIR)
