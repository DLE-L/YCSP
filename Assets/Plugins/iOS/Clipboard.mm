#import <UIKit/UIKit.h>

extern "C" {
    const char* _GetClipboardText() {
        UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
        NSString *string = pasteboard.string ?: @"";
        return strdup([string UTF8String]);
    }
}
