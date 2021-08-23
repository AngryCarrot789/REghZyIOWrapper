#include "DString.h"
#include "CharBuffer.h"

CharBuffer* buffer;

void processNewLine();
void sendPacketFailed(DString& str);

void processPacket00(int meta);
void processPacket01(int meta);
void processPacket02(int meta);
void processPacket03(int meta);

void setup() {
	buffer = new CharBuffer(128);
	Serial.begin(9600);
}

void loop() {
	while (Serial.available() > 0) {
		char read = (char)Serial.read();
		if (read == '\r') {
			continue;
		}
		if (read == '\n') {
			processNewLine();
			buffer->clear();
			continue;
		}

		buffer->appendChar(read);
	}
}

void processNewLine() {
	int readable = buffer->readable();
	if (readable < 4) {
		sendPacketFailed(DString("Stuff").ref());
		return;
	}

	int id = buffer->readint(2);
	if (id < 0 || id > 99) {
		sendPacketFailed(DString("ID Out of range (").append(id).append(")"));
		return;
	}

	int meta = buffer->readint(2);
	if (meta < 0 || meta > 99) {
		sendPacketFailed(DString("Meta Out of range (").append(meta).append(")"));
		return;
	}

	switch (id) {
		case 0: processPacket00(meta); break;
		case 1: processPacket01(meta); break;
		case 2: processPacket02(meta); break;
		case 3: processPacket03(meta); break;
	}
}

void processPacket00(int meta) {
	int dest = buffer->readint(1);
	if (dest != 1) {
		return;
	}
	char c = buffer->readChar();
	if (c == '.') {
		int reqId = buffer->readint(2);
		c = buffer->readChar();
		if (c == '.') {
			int code = buffer->readint(1);
			if (code == 1) {
				DString str = "0000";
				str.append("3.");
				if (reqId > 9) {
					str.append(reqId);
				}
				else {
					str.append("0");
					str.append(reqId);
				}

				str.append(".");
				str.append(code);
				str.append(".Arduino/Atmega328p - REghZy Software V0-1-246");
				Serial.println(str.toString());
				str.deleteBuffer();
			}
			else {
				sendPacketFailed(DString("Code != 1").ref());
			}
		}
		else {
			sendPacketFailed(DString("DOT 2 Missing").ref());
		}
	}
	else {
		sendPacketFailed(DString("DOT 1 Missing").ref());
	}
}

void processPacket01(int meta) { }
void processPacket02(int meta) { }
void processPacket03(int meta) { }

// Autodeletes the given string buffer for you
void sendPacketFailed(DString& str) {
	Serial.println();
	Serial.print("0900");
	Serial.print(str.toString());
	str.deleteBuffer();
	Serial.println(" || ");
	//Serial.println(((const char*)buffer->c_str()));
	//str = " || Index = ";
	//str.append(buffer->getReadIndex());
	//Serial.println(str.toString());
	//str.deleteBuffer();
}