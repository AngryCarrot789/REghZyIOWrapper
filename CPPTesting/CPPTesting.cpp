#include <iostream>
#include "StringBuilder.h"

int main() {
    std::cout << "Hello World!\n";

    StringBuilder sb = StringBuilder("123456789");
    sb.insert(sb.indexOf("345"), "helo", 4);
    std::cout << "'" << sb.toString() << "'\n";
}
