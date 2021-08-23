#ifndef __STRING_BUILDER
#define __STRING_BUILDER

#include <stdlib.h>
#include <math.h>

// A buffer than autoresizes itself, and supports appending strings and integers
class StringBuilder {
public:
	static inline int stringlen(const char* str, int startIndex) {
		int c = 0;
		while (str[startIndex++] != 0) { c++; }
		return c;
	}

	static int intlen(const char* str, int startIndex) {
		int count = 0;
		char c = str[startIndex++];
		while (c >= '0' && c <= '9') {
			count++;
			c = str[startIndex++];
		}
		return count;
	}

	int stoi(const char* str, int len, int startIndex) {
		int i = 0, j = startIndex, end = j + len;
		while (j < end) {
			i = (i * 10) + (str[j++] - '0');
		}
		return i;
	}

	int itostr(int value, char* str, int startIndex) {
		int i = ((int)log10((double)value)) + startIndex;
		int len = 0;
		while (value > 0) {
			str[i--] = ((value % 10) + '0');
			value = value / 10;
			len++;
		}
		return len;
	}

	void charset(char* buf, int value, int len) {
		for (int i = 0; i < len; i++) {
			buf[i] = value;
		}
	}

	// Creates an empty StringBuilder with the given capacity
	StringBuilder(int capacity) {
		mCapacity = capacity;
		mNextIndex = 0;
		createCapacityBuffer();
		setLastAsNull();
	}

	// Creates a StringBuilder, and appends the given char array (with the given size (srcLen)) to this stringbuilder
	StringBuilder(const char* src, int srcLen, int capacity) {
		mCapacity = capacity;
		mNextIndex = srcLen;
		createCapacityBuffer();
		for (int i = 0; i < srcLen; i++) {
			mBuffer[i] = src[i];
		}

		setLastAsNull();
	}

	// Creates a StringBuilder, and concatinates 2 char arrays
	StringBuilder(const char* a, const int lenA, const char* b, const int lenB) {
		mCapacity = (lenA + lenB);
		mNextIndex = 0;
		createCapacityBuffer();
		for (int i = 0; i < lenA; i++) {
			mBuffer[i] = a[i];
		}
		for (int i = lenA, j = 0; i < lenB; i++, j++) {
			mBuffer[i] = b[j];
		}

		setLastAsNull();
	}

	// Creates a StringBuilder, and appends the given string
	StringBuilder(const char* src) {
		int len = stringlen(src, 0);
		mCapacity = len;
		mNextIndex = len;
		createCapacityBuffer();
		for (int i = 0; i < len; i++) {
			mBuffer[i] = src[i];
		}
		setLastAsNull();
	}

	// Appends a substring of the given char array (starting at the startIndex (inclusive) and ending at the endIndex (exclusive). Similar to java's substring)
	StringBuilder& appendSubstring(const char* str, int startIndex, int endIndex) {
		ensureBufferSize(endIndex - startIndex);
		for (int i = startIndex; i < endIndex; i++) {
			mBuffer[mNextIndex++] = str[i];
		}

		return ref();
	}

	// Appends a string (with the given size) to this StringBuilder
	StringBuilder& appendString(const char* str, int len) {
		ensureBufferSize(len);
		for (int i = 0; i < len; i++) {
			mBuffer[mNextIndex++] = str[i];
		}

		return ref();
	}

	StringBuilder& appendString(const char* str) {
		return appendString(str, stringlen(str, 0));
	}

	StringBuilder& appendChar(const char c) {
		ensureBufferSize(1);
		mBuffer[mNextIndex++] = c;
	}

	// Appends an integer
	StringBuilder& appendInt(int value) {
		char c[10];
		charset(c, 0, 10);
		int intLen = itostr(value, c, 0);
		return appendString(c, intLen);
	}

	StringBuilder& appendLine() {
		return appendChar('\n');
	}

	// Returns the number of characters that have been appended to this StringBuilder
	int getSize() {
		return this->mNextIndex;
	}

	// Returns the capacity of the internal buffer (not including the null-termination char)
	int getCapacity() {
		return this->mCapacity;
	}

	// Returns the pointer to the internal null-terminated char buffer (aka c_str)
	char* toString() {
		setNonCharToNull();
		return this->mBuffer;
	}

	// Returns a reference to this StringBuilder
	StringBuilder& ref() {
		return *this;
	}

	// Returns the pointer to this StringBuilder
	StringBuilder* ptr() {
		return this;
	}

	inline StringBuilder operator+(const StringBuilder& right) { return StringBuilder(mBuffer, mNextIndex, right.mBuffer, right.mNextIndex); }

	StringBuilder operator+(const char value) {
		StringBuilder sb = StringBuilder(mBuffer, mNextIndex, mNextIndex + 1);
		sb.appendChar(value);
		return sb;
	}

	StringBuilder operator+(const int value) {
		char c[10];
		charset(c, 0, 10);
		int intLen = itostr(value, c, 0);
		return StringBuilder(mBuffer, mNextIndex, c, intLen);
	}

	inline StringBuilder& operator+=(const StringBuilder& right) { return appendString(right.mBuffer, right.mNextIndex); }
	inline StringBuilder& operator<<(const StringBuilder& right) { return appendString(right.mBuffer, right.mNextIndex); }

	inline StringBuilder& operator+=(const int value) { return appendInt(value); }
	inline StringBuilder& operator<<(const int value) { return appendInt(value); }

	inline StringBuilder& operator+=(const char value) { return appendChar(value); }
	inline StringBuilder& operator<<(const char value) { return appendChar(value); }

	// Ensures the internal buffer can fit the given number of extra characters (extraSize)
	// Returns true if the buffer didn't require a resize, or if it was successfully resized
	// Returns false if the buffer failed to resize (possibly due to memory fragmentation)
	bool ensureBufferSize(int extraSize) {
		if (requireResize(extraSize)) {
			return resizeBuffer(mCapacity + extraSize, true);
		}

		return true;
	}

	// Checks if the internal buffer needs to be resized to fit the given number of extra characters (extraSize)
	bool requireResize(const int extraSize) {
		if ((extraSize + mNextIndex) > mCapacity) {
			return true;
		}

		return false;
	}

private:
	// Sets the last character in this StringBuilder as 0 (aka a null character, allowing strlen to be used)
	void setLastAsNull() {
		setNonCharToNull();
		this->mBuffer[this->mCapacity] = 0;
	}

	// Resizes the internal buffer to the given size, optionally copying the old buffer into the new one
	bool resizeBuffer(const int newSize, const bool copyBuffer) {
		if (copyBuffer) {
			// hopefully prevents lots of cases of heap fragmentation on 
			// things that dont have much ram... like arduinos
			void* newBuffer = realloc(mBuffer, newSize);
			if (newBuffer == nullptr) {
				return false;
			}

			mCapacity = newSize;
			mBuffer = (char*)newBuffer;
			setLastAsNull();
			return true;
		}
		else {
			delete[](mBuffer);
			mBuffer = new char[newSize + 1];
			mCapacity = newSize;
			mNextIndex = 0;
			setLastAsNull();
			return true;
		}
	}

	// Sets all of the chars including (and past) the next write index, to null
	// So that toString() wont return extra stuff
	void setNonCharToNull() {
		for (int i = mNextIndex; 
			i < mCapacity; 
			i++) {
			mBuffer[i] = 0;
		}
	}

	void createCapacityBuffer() {
		mBuffer = new char[mCapacity + 1];
	}

private:
	int mCapacity;
	int mNextIndex;
	char* mBuffer;
};

#endif // !__STRING_BUILDER