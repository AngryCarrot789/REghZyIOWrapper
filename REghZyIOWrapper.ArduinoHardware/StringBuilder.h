#ifndef __STRING_BUILDER
#define __STRING_BUILDER

#define BUFFER_RESIZE_SUCCESS 1
#define BUFFER_RESIZE_UNCHANCED 0
#define BUFFER_RESIZE_ERR -1

#include <stdlib.h>
#include <math.h>

// A buffer than autoresizes itself, and supports appending strings and integers
class StringBuilder {
public:
	static inline int stringlen(const char* str, const int startIndex) {
		int i = startIndex;
		int c = 0;
		while (str[i++] != 0) {
			c++;
		}

		return c;
	}

	static inline int stringlen(const char* str) {
		return stringlen(str, 0);
	}

	static int intlen(const char* str, const int startIndex) {
		int i = startIndex;
		int len = 0;
		char c = str[i++];
		while (c >= '0' && c <= '9') {
			len++;
			c = str[i++];
		}

		return (len < 1) ? 1 : len;
	}

	static int intlen(const int value) {
		int len = 0;
		int temp = 1;
		while (temp <= value) {
			len++;
			temp *= 10;
		}

		return (len < 1) ? 1 : len;
	}

	static int stoi(const char* str, const int len, const int startIndex) {
		int i = 0, j = startIndex, end = j + len;
		while (j < end) {
			i = (i * 10) + (str[j++] - '0');
		}

		return i;
	}

	static int itostr(const int value, char* str, const int startIndex) {
		if (value == 0) {
			str[startIndex] = '0';
			return 1;
		}
		else {
			int val = value;
			int i = (intlen(val) - 1) + startIndex;
			int len = 0;
			while (val > 0) {
				str[i--] = ((val % 10) + '0');
				val = val / 10;
				len++;
			}
			return (len < 1) ? 1 : len;
		}
	}

	// Sets all the charactes in the given buffer (starting at 0, for the given len) to the given char value
	static void charset(char* buf, const char value, const int len) {
		for (int i = 0; i < len; i++) {
			buf[i] = value;
		}
	}

	StringBuilder(void) {
		bufferCapacity = 16;
		nextIndex = 0;
		createCapacityBuffer();
		setLastAsNull();
	}

	// Creates an empty StringBuilder with the given capacity
	StringBuilder(const int capacity) {
		bufferCapacity = capacity;
		nextIndex = 0;
		createCapacityBuffer();
		setLastAsNull();
	}

	// Creates a StringBuilder, and appends the given char array (with the given size (srcLen)) to this stringbuilder
	StringBuilder(const char* src, const int srcLen, const int capacity) {
		bufferCapacity = capacity;
		nextIndex = srcLen;
		createCapacityBuffer();
		for (int i = 0; i < srcLen; i++) {
			m_buffer[i] = src[i];
		}

		setLastAsNull();
	}

	// Creates a StringBuilder, and concatinates 2 char arrays
	StringBuilder(const char* a, const int lenA, const char* b, const int lenB) {
		bufferCapacity = (lenA + lenB);
		nextIndex = 0;
		createCapacityBuffer();
		for (int i = 0; i < lenA; i++) {
			m_buffer[i] = a[i];
		}
		for (int i = lenA, j = 0; i < lenB; i++, j++) {
			m_buffer[i] = b[j];
		}

		setLastAsNull();
	}

	// Creates a StringBuilder, and appends the given string
	StringBuilder(const char* src) {
		int len = stringlen(src, 0);
		bufferCapacity = len;
		nextIndex = len;
		createCapacityBuffer();
		for (int i = 0; i < len; i++) {
			m_buffer[i] = src[i];
		}

		setLastAsNull();
	}

	~StringBuilder() {
		deleteBuffer();
	}

	// Appends a substring of the given char array (starting at the startIndex (inclusive) and ending at the endIndex (exclusive). Similar to java's substring)
	StringBuilder& appendSubstring(const char* str, const int startIndex, const int endIndex) {
		if (ensureBufferSize(endIndex - startIndex) == BUFFER_RESIZE_SUCCESS) {
			for (int i = startIndex; i < endIndex; i++) {
				m_buffer[nextIndex++] = str[i];
			}
		}

		return ref();
	}

	// Appends a string (with the given size) to this StringBuilder
	StringBuilder& appendString(const StringBuilder& sb) {
		if (ensureBufferSize(sb.nextIndex) == BUFFER_RESIZE_SUCCESS) {
			for (int i = 0; i < sb.nextIndex; i++) {
				m_buffer[nextIndex++] = sb.m_buffer[i];
			}
		}

		return ref();
	}

	// Appends a string (with the given size) to this StringBuilder
	StringBuilder& appendString(const char* str, const int len) {
		if (ensureBufferSize(len) == BUFFER_RESIZE_SUCCESS) {
			for (int i = 0; i < len; i++) {
				m_buffer[nextIndex++] = str[i];
			}
		}

		return ref();
	}

	StringBuilder& appendString(const char* str) {
		return appendString(str, stringlen(str, 0));
	}

	StringBuilder& appendChar(const char c) {
		if (ensureBufferSize(1) == BUFFER_RESIZE_SUCCESS) {
			m_buffer[nextIndex++] = c;
		}

		return ref();
	}

	// Appends an integer
	StringBuilder& appendInt(const int value) {
		if (value == 0) {
			return appendString("0");
		}
		else {
			char c[10] = { 0 };
			charset(c, 0, 10);
			int intLen = itostr(value, c, 0);
			return appendString(c, intLen);
		}
	}

	StringBuilder& appendLine() {
		return appendChar('\n');
	}

	StringBuilder substring(const int startIndex, const int endIndex) {
		int len = endIndex - startIndex;
		StringBuilder sb = StringBuilder(len);
		sb.appendSubstring(m_buffer, startIndex, endIndex);
		return sb;
	}

	int indexOf(const char value, const int startIndex) {
		for (int i = startIndex; i < nextIndex; i++) {
			if (m_buffer[i] == value) {
				return i;
			}
		}

		return -1;
	}

	int indexOf(const char value) {
		return indexOf(value, 0);
	}

	int indexOf(const char* value, const int len, const int startIndex) {
		char first = value[0];
		for (int i = startIndex; i < nextIndex; i++) {
			if (m_buffer[i] == first) {
				for (int j = 0, k = i; j < len && k < nextIndex; j++, k++) {
					if (m_buffer[k] = value[j]) {
						return i;
					}
				}
			}
		}

		return -1;
	}

	int indexOf(const char* value, const int len) {
		return indexOf(value, len, 0);
	}

	int indexOf(const char* value) {
		return indexOf(value, stringlen(value, 0), 0);
	}

	char charAt(const int index) {
		return m_buffer[index];
	}

	// Inserts the given string (with the given length) starting at the given index
	// Say this StringBuilder contained "hello", calling insert(2, "XD, 2)
	// would result in the StringBuilder containing "heXDllo" (essentially pushing everything
	// at index 2 and past, to the right and putting the value inbetween)
	StringBuilder& insert(const int index, const char* str, const int len) {
		copyBlockRight(index, len);
		for (int i = index, end = index + len, j = 0; i < end; i++, j++) {
			m_buffer[i] = str[j];
		}

		return ref();
	}

	StringBuilder& insert(const int index, const char* str) {
		int len = stringlen(str, 0);
		copyBlockRight(index, len);
		for (int i = index, end = index + len, j = 0; i < end; i++, j++) {
			m_buffer[i] = str[j];
		}

		return ref();
	}

	// Copies all the characters at and past the given start index to the right (by the given amount (gap)), filling the gaps with a null character
	// If gap is 0, nothing happens
	StringBuilder& copyBlockRight(const int start, const int gap) {
		if (gap < 1) {
			return ref();
		}

		if (ensureBufferSize(gap) == BUFFER_RESIZE_SUCCESS) {
			for (int i = (nextIndex - 1); i >= start; i--) {
				char c = m_buffer[i];
				m_buffer[i + gap] = c;
			}

			nextIndex += gap;
		}

		return ref();
	}

	char* startPtr() {
		return m_buffer;
	}

	char* endPtr() {
		return m_buffer + nextIndex;
	}

	// Returns the number of characters that have been appended to this StringBuilder
	int getSize() {
		return nextIndex;
	}

	// Returns the capacity of the internal buffer (not including the null-termination char)
	int getCapacity() {
		return bufferCapacity;
	}

	// Returns the pointer to the internal null-terminated char buffer (aka c_str)
	char* toString() {
		setNonCharToNull();
		return m_buffer;
	}

	// Returns a reference to this StringBuilder
	StringBuilder& ref() {
		return *this;
	}

	// Returns the pointer to this StringBuilder
	StringBuilder* ptr() {
		return this;
	}

	inline StringBuilder operator+(const StringBuilder& right) { return StringBuilder(m_buffer, nextIndex, right.m_buffer, right.nextIndex); }

	StringBuilder operator+(const char value) {
		StringBuilder sb = StringBuilder(m_buffer, nextIndex, nextIndex + 1);
		sb.appendChar(value);
		return sb;
	}

	StringBuilder operator+(const int value) {
		char c[10];
		charset(c, 0, 10);
		int intLen = itostr(value, c, 0);
		return StringBuilder(m_buffer, nextIndex, c, intLen);
	}

	inline StringBuilder& operator<<(const StringBuilder& right) { return appendString(right.m_buffer, right.nextIndex); }
	inline StringBuilder& operator<<(const char* value) { return appendString(value); }
	inline StringBuilder& operator<<(const char value) { return appendChar(value); }
	inline StringBuilder& operator<<(const int value) { return appendInt(value); }
	inline StringBuilder& operator+=(const StringBuilder& right) { return appendString(right.m_buffer, right.nextIndex); }
	inline StringBuilder& operator+=(const char value) { return appendChar(value); }
	inline StringBuilder& operator+=(const int value) { return appendInt(value); }
	inline StringBuilder operator=(const char* value) { return StringBuilder(value); }

	// Clears the buffer (setting everything to 0) and sets the write index to 0
	StringBuilder& clear() {
		for (int i = 0; i < nextIndex; i++) {
			m_buffer[i] = 0;
		}

		nextIndex = 0;
	}

	// Does not clear the actual buffer, simply resets the write index to 0
	StringBuilder& resetIndex() {
		nextIndex = 0;
	}

	// Ensures the internal buffer can fit the given number of extra characters (extraSize)
	// Returns true if the buffer didn't require a resize, or if it was successfully resized
	// Returns false if the buffer failed to resize (possibly due to memory fragmentation)
	int ensureBufferSize(const int extraSize) {
		if (requireResize(extraSize)) {
			if (resizeBuffer(bufferCapacity + extraSize)) {
				return BUFFER_RESIZE_SUCCESS;
			}

			return BUFFER_RESIZE_ERR;
		}

		return BUFFER_RESIZE_UNCHANCED;
	}

	// Checks if the internal buffer needs to be resized to fit the given number of extra characters (extraSize)
	inline bool requireResize(const int extraSize) {
		return (extraSize + nextIndex) > bufferCapacity;
	}

	// Deletes the internal buffer
	void deleteBuffer() {
		delete[](m_buffer);
	}

private:
	// Sets the last character in this StringBuilder as 0 (aka a null character, allowing strlen to be used)
	void setLastAsNull() {
		setNonCharToNull();
		m_buffer[bufferCapacity] = 0;
	}

	// Resizes the internal buffer to the given size, optionally copying the old buffer into the new one
	bool resizeBuffer(const int newSize) {
		// hopefully prevents heap fragmentation on 
		// things that dont have much ram... like arduinos
		void* newBuffer = realloc(m_buffer, newSize + 1);
		if (newBuffer == nullptr) {
			return false;
		}

		bufferCapacity = newSize;
		m_buffer = (char*)newBuffer;
		setLastAsNull();
		return true;
	}

	// Sets all of the chars including (and past) the next write index, to null
	// So that toString() wont return extra stuff
	void setNonCharToNull() {
		for (int i = nextIndex, end = (bufferCapacity + 1); i < end; i++) {
			m_buffer[i] = 0;
		}
	}

	// Creates the buffer with the capacity + 1
	// DOES NOT DELETE THE OLD BUFFER!
	// msg to me; delete it or rip memory
	void createCapacityBuffer() {
		m_buffer = new char[bufferCapacity + 1];
	}

private:
	// The capacity of the internal buffer
	int bufferCapacity;

	// The index where the next char is to be set (and also how many chars have been written)
	int nextIndex;

	// The pointer to the buffer
	char* m_buffer;
};

#endif // !__STRING_BUILDER