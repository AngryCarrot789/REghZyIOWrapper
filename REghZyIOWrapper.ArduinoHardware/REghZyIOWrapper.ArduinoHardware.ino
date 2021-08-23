#include "CharBuffer.h"
#include "StringBuilder.h"

CharBuffer* buffer;

StringBuilder errBuffer = StringBuilder(256);

void processNewLine();
void sendPacketFailed();

void processPacket00(int meta);
void processPacket01(int meta);
void processPacket02(int meta);
void processPacket03(int meta);

void sendPacket(int id, int meta, StringBuilder& dataAutoDeleted);
void sendMessage(StringBuilder& msg);

void setup() {
	buffer = new CharBuffer(128);
	Serial.begin(9600);
	pinMode(3, HIGH);
	pinMode(4, HIGH);
	pinMode(5, HIGH);
	pinMode(6, HIGH);
	pinMode(7, HIGH);
	pinMode(8, HIGH);
	pinMode(9, HIGH);
	pinMode(10, HIGH);
	pinMode(11, HIGH);
	pinMode(12, HIGH);
	pinMode(13, HIGH);
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
	errBuffer.clear();
	int readable = buffer->readable();
	if (readable < 4) {
		errBuffer.appendString("Readable chars < 4");
		sendPacketFailed();
		return;
	}

	int id = buffer->readint(2);
	if (id < 0 || id > 99) {
		errBuffer.appendString("ID Out of Range (it was ").appendInt(id).appendChar(')');
		sendPacketFailed();
		return;
	}

	int meta = buffer->readint(2);
	if (meta < 0 || meta > 99) {
		errBuffer.appendString("Meta Out of Range (it was ").appendInt(id).appendChar(')');
		sendPacketFailed();
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
				errBuffer.appendString("Code != 1");
				sendPacketFailed();
			}
		}
		else {
			errBuffer.appendString("2nd dot = bad");
			sendPacketFailed();
		}
	}
	else {
		errBuffer.appendString("1st dot = bad");
		sendPacketFailed();
	}
}

void processPacket01(int meta) {
	char c = buffer->readChar();
	if (c == 'H') {
		digitalWrite(meta, HIGH);
		sendMessage(StringBuilder(20).appendString("Pin ").appendInt(meta).appendString(" set to HIGH"));
	}
	else if (c == 'L') {
		digitalWrite(meta, LOW);
		sendMessage(StringBuilder(20).appendString("Pin ").appendInt(meta).appendString(" set to LOW"));
	}
	else {
		errBuffer.appendString("The char '").appendChar(c).appendString("' was unknown! Pin = ").appendInt(meta);
		sendPacketFailed();
	}
}

void processPacket02(int meta) { }
void processPacket03(int meta) { }

// Autodeletes the given string buffer for you
void sendPacketFailed() {
	Serial.println();
	Serial.print("0900");
	Serial.println(errBuffer.appendString(". Index = ").appendInt(buffer->getReadIndex()).toString());
	errBuffer.clear();
}

void sendPacket(int id, int meta, StringBuilder& dataAutoDeleted) {
	StringBuilder sb = StringBuilder(dataAutoDeleted.getSize() + 4);
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

	sb.appendString(dataAutoDeleted);
	Serial.println(sb.toString());
	dataAutoDeleted.deleteBuffer();
}

void sendMessage(StringBuilder& msg) {
	sendPacket(8, 0, msg);
}