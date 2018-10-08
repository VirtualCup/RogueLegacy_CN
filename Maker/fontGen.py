# coding=UTF-8
import codecs
import sys
from xml.sax.saxutils import escape

reload(sys);
sys.setdefaultencoding('utf8');

chars = [];

def add_chars(text):
	text = text.strip()
	for s in text: 
		if(not escape(s) in chars): chars.append(escape(s));

def add_western():
	specil_chars = u" !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ¡«®°·º»¿ÀÁÃÄÇÉÊÍÓÖÚÜßàáâãäçèéêíîïñóôõöùúûüąĆćĘęŁłŃńœŚśźŻżАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя–";
	add_chars(specil_chars);

def add_file_text(filename):
	with codecs.open(filename, 'r', 'utf-8') as f:
		for line in f:
			add_chars(line);

def gen_spritefont(filename):
	print("font gen start!");
	
	f = codecs.open(filename, 'w', 'utf-8');
	
	f.write("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\n");
	f.write("<XnaContent xmlns:Graphics=\"Microsoft.Xna.Framework.Content.Pipeline.Graphics\">" + "\n");
	f.write("  <Asset Type=\"Graphics:FontDescription\">" + "\n");
	f.write("    <FontName>微软雅黑</FontName>" + "\n");
	f.write("    <Size>32</Size>" + "\n");
	f.write("    <Spacing>0</Spacing>" + "\n");
	f.write("    <UseKerning>true</UseKerning>" + "\n");
	f.write("    <Style>Regular</Style>" + "\n");
	f.write("    <DefaultCharacter>*</DefaultCharacter>" + "\n");
	f.write("    <CharacterRegions>" + "\n");
	
	chars.sort();
	for c in chars:
		f.write("      <CharacterRegion><Start>" + c + "</Start><End>" + c + "</End></CharacterRegion>" + "\n");
	
	f.write("    </CharacterRegions>" + "\n");
	f.write("  </Asset>" + "\n");
	f.write("</XnaContent>" + "\n");
	
	f.close();
	
	print("font gen end!");
	
# Get all characters.
#add_file_text("RogueCastle.Resources.LocStrings.zh.resx");

add_western();
add_file_text("RogueCastle.Resources.LocStrings.zh.resx");
gen_spritefont("NotoSansSC.spritefont");