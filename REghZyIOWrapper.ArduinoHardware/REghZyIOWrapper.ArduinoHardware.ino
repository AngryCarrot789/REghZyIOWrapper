#include "CharBuffer.h"
#include "StringBuilder.h"
CharBuffer* buffer;

void processNewLine();
void sendPacketFailed(const char* msg);

void processPacket00(int meta);
void processPacket01(int meta);
void processPacket02(int meta);
void processPacket03(int meta);

void sendPacket(int id, int meta, const char* data);
void sendMessage(const char* msg);

void setup() {
	buffer = new CharBuffer(128);
	Serial.begin(9600);
	pinMode(3, OUTPUT);
	pinMode(4, OUTPUT);
	pinMode(5, OUTPUT);
	pinMode(6, OUTPUT);
	pinMode(7, OUTPUT);
	pinMode(8, OUTPUT);
	pinMode(9, OUTPUT);
	pinMode(10, OUTPUT);
	pinMode(11, OUTPUT);
	pinMode(12, OUTPUT);
	pinMode(13, OUTPUT);
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
		sendPacketFailed("Readable chars < 4");
		return;
	}

	int id = buffer->readint(2);
	if (id < 0 || id > 99) {
		sendPacketFailed(StringBuilder(32).appendString("ID Out of Range (it was ").appendInt(id).appendChar(')').toString());
		return;
	}

	int meta = buffer->readint(2);
	if (meta < 0 || meta > 99) {
		sendPacketFailed(StringBuilder(34).appendString("Meta Out of Range (it was ").appendInt(meta).appendChar(')').toString());
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
				StringBuilder str = "0000";
				str.appendString("3.");
				if (reqId > 9) {
					str.appendInt(reqId);
				}
				else {
					str.appendString("0");
					str.appendInt(reqId);
				}

				str.appendString(".");
				str.appendInt(code);
				str.appendString(".Arduino/Atmega328p - REghZy Software V0-1-246");
				Serial.println(str.toString());
				str.deleteBuffer();
			}
			else {
				sendPacketFailed("Code != 1");
			}
		}
		else {
			sendPacketFailed("2nd dot = bad");
		}
	}
	else {
		sendPacketFailed("1st dot = bad");
	}
}

void processPacket01(int meta) {
	char c = buffer->readChar();
	if (c == 'H') {
		digitalWrite(meta, HIGH);
		sendMessage(StringBuilder(20).appendString("Pin ").appendInt(meta).appendString(" set to HIGH").toString());
	}
	else if (c == 'L') {
		digitalWrite(meta, LOW);
		sendMessage(StringBuilder(20).appendString("Pin ").appendInt(meta).appendString(" set to LOW").toString());
	}
	else {
		sendPacketFailed(StringBuilder(48).appendString("The char '").appendChar(c).appendString("' was unknown! Pin = ").appendInt(meta).toString());
	}
}

void processPacket02(int meta) { }
void processPacket03(int meta) { }

// Autodeletes the given string buffer for you
void sendPacketFailed(const char* errMsg) {
	Serial.println();
	Serial.print("0900");
	Serial.println(StringBuilder(StringBuilder::stringlen(errMsg) + 14).appendString(errMsg).appendString(". Index = ").appendInt(buffer->getReadIndex()).toString());
}

void sendPacket(int id, int meta, const char* data) {
	StringBuilder sb = StringBuilder(StringBuilder::stringlen(data) + 4);
	if (id > 9) {
		sb.appendInt(id);
	}
	else {
		sb.appendString("0").appendInt(id);
	}
	if (meta > 9) {
		sb.appendInt(meta);
	}
	else {
		sb.appendString("0").appendInt(meta);
	}

	sb.appendString(data);
	Serial.println(sb.toString());
}

void sendMessage(const char* msg) {
	sendPacket(8, 0, msg);
}