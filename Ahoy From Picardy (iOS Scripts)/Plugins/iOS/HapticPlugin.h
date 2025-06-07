// HapticPlugin.h

#import <Foundation/Foundation.h>

#ifdef __cplusplus
extern "C" {
#endif

#if TARGET_OS_IOS
    #import <UIKit/UIKit.h> // Import UIKit inside the iOS check

    bool _IsHapticsSupported();
    void _PlayHaptic(int hapticType);
#endif // TARGET_OS_IOS

#ifdef __cplusplus
} // Close the extern "C" block
#endif
