#!/usr/bin/env python3
import os
import re
import xml.etree.ElementTree as ET
import glob
from xml.dom import minidom
import datetime

# Configuration
DRAWIO_FILE = "UMLs/Merge2-BattleField.drawio"
OUTPUT_FILE = "UMLs/Merge2-BattleField-Updated.drawio"  # New file to avoid modifying the original
SOURCE_DIR = "Assets"
CS_FILE_PATTERN = "**/*.cs"

# Regular expressions for parsing C# code
CLASS_PATTERN = r"(?:public|private|protected|internal)\s+(?:abstract|sealed|static)?\s*class\s+(\w+)(?:\s*:\s*(\w+))?"
INTERFACE_PATTERN = r"(?:public|private|protected|internal)\s+interface\s+(\w+)(?:\s*:\s*(\w+))?"
METHOD_PATTERN = r"(?:public|private|protected|internal)\s+(?:static|virtual|abstract|override)?\s+(\w+)\s+(\w+)\s*\(([^)]*)\)"
FIELD_PATTERN = r"(?:public|private|protected|internal)\s+(?:readonly|static|const)?\s+(\w+)\s+(\w+)\s*[;=]"
PROPERTY_PATTERN = r"(?:public|private|protected|internal)\s+(?:virtual|abstract|override)?\s+(\w+)\s+(\w+)\s*\{[^}]*\}"

class CSharpParser:
    def __init__(self):
        self.classes = {}
        
    def parse_file(self, file_path):
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
            
        # Extract namespace
        namespace_match = re.search(r"namespace\s+([^{]+)", content)
        namespace = namespace_match.group(1).strip() if namespace_match else ""
        
        # Find classes
        for match in re.finditer(CLASS_PATTERN, content):
            class_name = match.group(1)
            base_class = match.group(2) if match.group(2) else ""
            
            # Create class entry
            full_class_name = f"{namespace}.{class_name}" if namespace else class_name
            self.classes[full_class_name] = {
                'name': class_name,
                'namespace': namespace,
                'type': 'class',
                'base': base_class,
                'methods': [],
                'fields': [],
                'properties': []
            }
            
        # Find interfaces
        for match in re.finditer(INTERFACE_PATTERN, content):
            interface_name = match.group(1)
            base_interface = match.group(2) if match.group(2) else ""
            
            # Create interface entry
            full_interface_name = f"{namespace}.{interface_name}" if namespace else interface_name
            self.classes[full_interface_name] = {
                'name': interface_name,
                'namespace': namespace,
                'type': 'interface',
                'base': base_interface,
                'methods': [],
                'fields': [],
                'properties': []
            }
            
        # Extract methods, fields, and properties for each class
        for class_name, class_info in self.classes.items():
            short_name = class_info['name']
            
            # Find methods
            for match in re.finditer(METHOD_PATTERN, content):
                return_type = match.group(1)
                method_name = match.group(2)
                parameters = match.group(3)
                
                # Check if this method belongs to this class (simple heuristic)
                # A more robust approach would parse the class body properly
                class_info['methods'].append({
                    'name': method_name,
                    'return_type': return_type,
                    'parameters': parameters
                })
                
            # Find fields
            for match in re.finditer(FIELD_PATTERN, content):
                field_type = match.group(1)
                field_name = match.group(2)
                
                class_info['fields'].append({
                    'name': field_name,
                    'type': field_type
                })
                
            # Find properties
            for match in re.finditer(PROPERTY_PATTERN, content):
                prop_type = match.group(1)
                prop_name = match.group(2)
                
                class_info['properties'].append({
                    'name': prop_name,
                    'type': prop_type
                })
                
    def get_classes(self):
        return self.classes


class DrawIOCreator:
    def __init__(self, output_file):
        self.output_file = output_file
        self.next_id = 2  # Start with ID 2 since 0 and 1 are reserved
        
        # Create basic XML structure
        self.mxfile = ET.Element("mxfile")
        self.mxfile.set("host", "app.diagrams.net")
        self.mxfile.set("modified", datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"))
        self.mxfile.set("agent", "Python Script")
        self.mxfile.set("version", "21.7.4")
        self.mxfile.set("type", "device")
        
        # Create diagram element
        self.diagram = ET.SubElement(self.mxfile, "diagram")
        self.diagram.set("id", "diagramId")
        self.diagram.set("name", "Merge2-BattleField UML")
        
        # Create mxGraphModel
        self.mxGraphModel = ET.Element("mxGraphModel")
        self.mxGraphModel.set("dx", "1326")
        self.mxGraphModel.set("dy", "798")
        self.mxGraphModel.set("grid", "1")
        self.mxGraphModel.set("gridSize", "10")
        self.mxGraphModel.set("guides", "1")
        self.mxGraphModel.set("tooltips", "1")
        self.mxGraphModel.set("connect", "1")
        self.mxGraphModel.set("arrows", "1")
        self.mxGraphModel.set("fold", "1")
        self.mxGraphModel.set("page", "1")
        self.mxGraphModel.set("pageScale", "1")
        self.mxGraphModel.set("pageWidth", "850")
        self.mxGraphModel.set("pageHeight", "1100")
        self.mxGraphModel.set("math", "0")
        self.mxGraphModel.set("shadow", "0")
        
        # Create root cell
        self.root = ET.SubElement(self.mxGraphModel, "root")
        
        # Add base cells (required by draw.io)
        cell0 = ET.SubElement(self.root, "mxCell")
        cell0.set("id", "0")
        
        cell1 = ET.SubElement(self.root, "mxCell")
        cell1.set("id", "1")
        cell1.set("parent", "0")
        
        # Store class IDs
        self.class_id_map = {}
    
    def add_class(self, class_info, x=100, y=100, width=200, height=100):
        # Create a style based on class type
        style = "swimlane;fontStyle=1;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;"
        if class_info['type'] == 'interface':
            style += "fillColor=#CCE5FF;"
        else:
            style += "fillColor=#E6FFCC;"
            
        # Create class cell
        class_id = str(self.next_id)
        self.next_id += 1
        
        # Calculate height based on content
        item_count = len(class_info['fields']) + len(class_info['properties']) + len(class_info['methods'])
        height = 26 + (item_count * 26) + 10  # Header + items + padding
        
        # Create class name label
        class_name = class_info['name']
        if class_info['namespace']:
            display_name = f"{class_info['namespace']}.{class_name}"
        else:
            display_name = class_name
            
        # Create the class element
        class_obj = ET.SubElement(self.root, "object")
        class_obj.set("id", class_id)
        class_obj.set("label", display_name)
        
        # Add class cell
        cell = ET.SubElement(class_obj, "mxCell")
        cell.set("style", style)
        cell.set("vertex", "1")
        cell.set("parent", "1")
        
        # Add geometry
        geometry = ET.SubElement(cell, "mxGeometry")
        geometry.set("x", str(x))
        geometry.set("y", str(y))
        geometry.set("width", str(width))
        geometry.set("height", str(height))
        geometry.set("as", "geometry")
        
        # Add fields section
        y_offset = 26  # Start after header
        
        # Add line separator
        separator_id = str(self.next_id)
        self.next_id += 1
        separator = ET.SubElement(self.root, "mxCell")
        separator.set("id", separator_id)
        separator.set("value", "")
        separator.set("style", "line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;")
        separator.set("vertex", "1")
        separator.set("parent", class_id)
        
        separator_geometry = ET.SubElement(separator, "mxGeometry")
        separator_geometry.set("y", str(y_offset))
        separator_geometry.set("width", str(width))
        separator_geometry.set("height", "8")
        separator_geometry.set("as", "geometry")
        
        y_offset += 8
        
        # Add fields
        for field in class_info['fields']:
            field_id = str(self.next_id)
            self.next_id += 1
            
            # Format field with visibility
            field_text = f"- {field['name']}: {field['type']}"
            
            field_cell = ET.SubElement(self.root, "mxCell")
            field_cell.set("id", field_id)
            field_cell.set("value", field_text)
            field_cell.set("style", "text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;")
            field_cell.set("vertex", "1")
            field_cell.set("parent", class_id)
            
            field_geometry = ET.SubElement(field_cell, "mxGeometry")
            field_geometry.set("y", str(y_offset))
            field_geometry.set("width", str(width))
            field_geometry.set("height", "26")
            field_geometry.set("as", "geometry")
            
            y_offset += 26
            
        # Add properties
        for prop in class_info['properties']:
            prop_id = str(self.next_id)
            self.next_id += 1
            
            # Format property
            prop_text = f"+ {prop['name']}: {prop['type']}"
            
            prop_cell = ET.SubElement(self.root, "mxCell")
            prop_cell.set("id", prop_id)
            prop_cell.set("value", prop_text)
            prop_cell.set("style", "text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;")
            prop_cell.set("vertex", "1")
            prop_cell.set("parent", class_id)
            
            prop_geometry = ET.SubElement(prop_cell, "mxGeometry")
            prop_geometry.set("y", str(y_offset))
            prop_geometry.set("width", str(width))
            prop_geometry.set("height", "26")
            prop_geometry.set("as", "geometry")
            
            y_offset += 26
            
        # Add methods
        for method in class_info['methods']:
            method_id = str(self.next_id)
            self.next_id += 1
            
            # Format method with parameters
            param_str = method['parameters'].strip()
            method_text = f"+ {method['name']}({param_str}): {method['return_type']}"
            
            method_cell = ET.SubElement(self.root, "mxCell")
            method_cell.set("id", method_id)
            method_cell.set("value", method_text)
            method_cell.set("style", "text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;")
            method_cell.set("vertex", "1")
            method_cell.set("parent", class_id)
            
            method_geometry = ET.SubElement(method_cell, "mxGeometry")
            method_geometry.set("y", str(y_offset))
            method_geometry.set("width", str(width))
            method_geometry.set("height", "26")
            method_geometry.set("as", "geometry")
            
            y_offset += 26
        
        # Store class ID
        self.class_id_map[f"{class_info['namespace']}.{class_name}" if class_info['namespace'] else class_name] = class_id
            
        return class_id
    
    def add_inheritance(self, child_id, parent_id):
        edge_id = str(self.next_id)
        self.next_id += 1
        
        edge = ET.SubElement(self.root, "mxCell")
        edge.set("id", edge_id)
        edge.set("value", "")
        edge.set("style", "endArrow=block;endSize=16;endFill=0;html=1;rounded=0;")
        edge.set("edge", "1")
        edge.set("source", child_id)
        edge.set("target", parent_id)
        edge.set("parent", "1")
        
        edge_geometry = ET.SubElement(edge, "mxGeometry")
        edge_geometry.set("relative", "1")
        edge_geometry.set("as", "geometry")
        
        return edge_id
    
    def save(self):
        # Update diagram content
        self.diagram.text = ET.tostring(self.mxGraphModel, encoding='unicode')
        
        # Pretty print XML for readability
        xml_str = ET.tostring(self.mxfile, encoding='unicode')
        pretty_xml = minidom.parseString(xml_str).toprettyxml(indent="  ")
        
        # Create parent directory if it doesn't exist
        os.makedirs(os.path.dirname(self.output_file), exist_ok=True)
        
        with open(self.output_file, 'w', encoding='utf-8') as f:
            f.write(pretty_xml)


def main():
    # Find all C# files in the project
    cs_files = glob.glob(os.path.join(SOURCE_DIR, CS_FILE_PATTERN), recursive=True)
    
    # Parse all C# files
    parser = CSharpParser()
    for file_path in cs_files:
        try:
            parser.parse_file(file_path)
            print(f"Parsed {file_path}")
        except Exception as e:
            print(f"Error parsing {file_path}: {str(e)}")
    
    # Get all classes from the parser
    classes = parser.get_classes()
    
    # Create a new DrawIO file
    creator = DrawIOCreator(OUTPUT_FILE)
    
    # Place classes in a grid layout
    cols = 3
    row, col = 0, 0
    
    # Add classes to the diagram
    for full_name, class_info in classes.items():
        # Calculate position for class
        x = 100 + (col * 300)
        y = 100 + (row * 250)
        
        # Add class to diagram
        creator.add_class(class_info, x, y)
        
        # Update grid position
        col += 1
        if col >= cols:
            col = 0
            row += 1
    
    # Add inheritance relationships
    for full_name, class_info in classes.items():
        base_name = class_info['base']
        
        # Check if base class exists in our diagram
        qualified_base_name = None
        if base_name:
            # Try to find the fully qualified name of the base class
            if f"{class_info['namespace']}.{base_name}" in creator.class_id_map:
                qualified_base_name = f"{class_info['namespace']}.{base_name}"
            elif base_name in creator.class_id_map:
                qualified_base_name = base_name
            
            # Add inheritance relationship if base class is found
            if qualified_base_name and full_name in creator.class_id_map:
                child_id = creator.class_id_map[full_name]
                parent_id = creator.class_id_map[qualified_base_name]
                creator.add_inheritance(child_id, parent_id)
    
    # Save the diagram
    creator.save()
    print(f"UML diagram created successfully: {OUTPUT_FILE}")


if __name__ == "__main__":
    print("UML Updater for Unity C# Project")
    print("=================================")
    print("This script will scan all C# files in your project and create a new UML diagram.")
    print(f"Source Directory: {SOURCE_DIR}")
    print(f"File Pattern: {CS_FILE_PATTERN}")
    print(f"Output File: {OUTPUT_FILE}")
    print("=================================")
    
    try:
        main()
        print("\nScript completed successfully!")
        print("You can now open your UML diagram in draw.io to see the results.")
    except Exception as e:
        print(f"\nError: {str(e)}")
        print("\nRequirements:")
        print("1. Python 3.6+ installed")
        print("2. XML libraries (included in standard Python)")
        print("3. Write permissions for the output directory")
        print("\nIf Python is not in PATH, run this script with the full path to Python:")
        print("e.g.: C:\\Python39\\python.exe update_uml.py")
