// HapticPlugin.m

#import "HapticPlugin.h" // Import its own header, as discussed
#if TARGET_OS_IOS
#import <UIKit/UIKit.h>
#endif

// This macro guard ensures extern "C" is only seen by a C++ compiler.
// When compiling as Objective-C, this block is skipped.
// Unity's build process for iOS plugins often uses Objective-C++ (.mm files)
// or handles the linkage correctly. This guard makes it explicit.
#ifdef __cplusplus
extern "C" {
#endif

    // Implementation of the function to check haptic support.
    bool _IsHapticsSupported()
    {
    #if TARGET_OS_IOS
        if (@available(iOS 10.0, *))
        {
            float systemVersion = [[[UIDevice currentDevice] systemVersion] floatValue];
            return (systemVersion >= 10.0f);
        }
        return false;
    #else
        return false;
    #endif
    }

    // Single function that takes an int to decide which haptic feedback to play.
    void _PlayHaptic(int hapticType)
    {
    #if TARGET_OS_IOS
        if (@available(iOS 10.0, *))
        {
            switch (hapticType)
            {
                case 0: // LightImpact
                {
                    UIImpactFeedbackGenerator *generator =
                        [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
                    [generator prepare];
                    [generator impactOccurred];
                    break;
                }
                case 1: // MediumImpact
                {
                    UIImpactFeedbackGenerator *generator =
                        [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
                    [generator prepare];
                    [generator impactOccurred];
                    break;
                }
                case 2: // HeavyImpact
                {
                    UIImpactFeedbackGenerator *generator =
                        [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
                    [generator prepare];
                    [generator impactOccurred];
                    break;
                }
                case 3: // Success
                {
                    UINotificationFeedbackGenerator *generator =
                        [[UINotificationFeedbackGenerator alloc] init];
                    [generator prepare];
                    [generator notificationOccurred:UINotificationFeedbackTypeSuccess];
                    break;
                }
                case 4: // Warning
                {
                    UINotificationFeedbackGenerator *generator =
                        [[UINotificationFeedbackGenerator alloc] init];
                    [generator prepare];
                    [generator notificationOccurred:UINotificationFeedbackTypeWarning];
                    break;
                }
                case 5: // Error
                {
                    UINotificationFeedbackGenerator *generator =
                        [[UINotificationFeedbackGenerator alloc] init];
                    [generator prepare];
                    [generator notificationOccurred:UINotificationFeedbackTypeError];
                    break;
                }
            }
        }
    #endif
    }

#ifdef __cplusplus
} // Close the extern "C" block
#endif
