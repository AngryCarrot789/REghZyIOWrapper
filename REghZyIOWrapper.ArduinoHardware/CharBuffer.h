#ifndef CHAR_BUFFER
#define CHAR_BUFFER

#include <math.h>

class CharBuffer {
public:
	CharBuffer(int size) {
		this->buffer = new char[size];
		DString::charset(this->buffer, 0, size);
		this->writeIndex = 0;
		this->readIndex = 0;
		this->length = size;
	}

	void appendChar(const char c) {
		int i = this->writeIndex + 1;
		if (i >= this->length) {
			return;
		}

		this->buffer[this->writeIndex] = c;
		this->writeIndex = i;
	}

	void appendString(const char* c, int len) {
		int i = this->writeIndex + len;
		if (i >= this->length) {
			return;
		}

		for (int j = this->writeIndex, k = 0; j < i; j++, k++) {
			this->buffer[j] = c[k];
		}

		this->writeIndex = i;
	}

	void appendint(const int intValue) {
		char c[10];
		DString::charset(c, 0, 10);
		DString::itostr(intValue, c);
		int nLen = DString::intlen(c);
		int endIndex = this->writeIndex + nLen;
		if (endIndex >= this->length) {
			return;
		}

		for (int j = this->writeIndex, k = 0; j < endIndex; j++, k++) {
			this->buffer[j] = c[k];
		}

		this->writeIndex = endIndex;
	}

	void appendBool(const bool value) {
		if (value) {
			appendChar('T');
		}
		else {
			appendChar('F');
		}
	}

	char readChar() {
		int i = this->readIndex + 1;
		if (i >= this->length) {
			return -1;
		}

		char c = this->buffer[this->readIndex];
		this->readIndex = i;
		return c;
	}

	// Reads an integer with the given length 
	// the length being the string length of the integer. 7 = 1, 23 = 2, 21484 = 5. 
	int readint(int len) {
		if (!canRead(len)) {
			return -1;
		}

		int i = DString::stoi(this->buffer, len, this->readIndex);
		this->readIndex += len;
		return i;
	}

	// Returns the number of valid characters there are to read
	// It iterates though all the chars at the internal index, and returns how many valid
	// chars are there until a null char is found
	int readableUntilNull() {
		int start = this->readIndex;
		int len = len = this->length;
		for (int i = start; i < len; i++) {
			if (this->buffer[i] == '\0') {
				return i;
			}
		}

		return len - start;
	}

	// Returns the number of readable numbers there are
	int readableInt() {
		int start = this->readIndex;
		int len = len = this->length;
		for (int i = start; i < len; i++) {
			char c = this->buffer[i];
			if (c < '0' || c > '9') {
				return i;
			}
		}

		return len - start;
	}

	// float readFloat() {
	// 	if (!canRead(4)) {
	// 		return -1;
	// 	}
	// 
	// 	return *(float*)((unsigned long)(((int)readChar()) | ((int)readChar() << 8) | ((int)readChar() << 16) | ((int)readChar() << 24)));
	// }

	bool canRead(int bytesToRead) {
		return (this->readIndex + bytesToRead) <= this->length;
	}

	int totalSize() {
		return this->length;
	}

	int bytesWritten() {
		return this->writeIndex;
	}

	int bytesRead() {
		return this->readIndex;
	}

	int readable() {
		return (this->length) - (this->readIndex);
	}

	int writable() {
		return this->length - this->writeIndex;
	}

	char* c_str() {
		return this->buffer;
	}

	int getReadIndex() {
		return this->readIndex;
	}

	void skipRead(int count) {
		this->readIndex += count;
	}

	void clear() {
		this->writeIndex = 0;
		this->readIndex = 0;
		DString::charset(this->buffer, 0, this->length);
	}

	// deletes the internal buffer using the free() method
	void deleteBuffer() {
		delete[] this->buffer;
		this->writeIndex = 0;
	}
private:
	int writeIndex;
	int readIndex;
	int length;
	char* buffer;
};

#endif