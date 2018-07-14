/*
  MPUSeries.cpp - Ardunity Arduino library
  Copyright (C) 2016 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "MPUSeries.h"
#include <stdio.h>
#include <math.h>
#include <avr/pgmspace.h>

//******************************************************************************
//* Defines & Declares
//******************************************************************************
// Filter configurations.
enum lpf_e
{
    INV_FILTER_256HZ_NOLPF2 = 0,
    INV_FILTER_188HZ,
    INV_FILTER_98HZ,
    INV_FILTER_42HZ,
    INV_FILTER_20HZ,
    INV_FILTER_10HZ,
    INV_FILTER_5HZ,
    INV_FILTER_2100HZ_NOLPF,
    NUM_FILTER
};

// Full scale ranges.
enum gyro_fsr_e
{
    INV_FSR_250DPS = 0,
    INV_FSR_500DPS,
    INV_FSR_1000DPS,
    INV_FSR_2000DPS,
    NUM_GYRO_FSR
};

// Full scale ranges.
enum accel_fsr_e
{
    INV_FSR_2G = 0,
    INV_FSR_4G,
    INV_FSR_8G,
    INV_FSR_16G,
    NUM_ACCEL_FSR
};

// Clock sources.
enum clock_sel_e
{
    INV_CLK_INTERNAL = 0,
    INV_CLK_PLL,
    NUM_CLK
};

// Low-power accel wakeup rates.
// MPU6050 or MPU9150
enum lp_accel_rate_e
{
    INV_LPA_1_25HZ,
    INV_LPA_5HZ,
    INV_LPA_20HZ,
    INV_LPA_40HZ
};
// MPU6500 or MPU9250
enum lp_accel_rate_e2
{
    INV_LPA2_0_3125HZ,
    INV_LPA2_0_625HZ,
    INV_LPA2_1_25HZ,
    INV_LPA2_2_5HZ,
    INV_LPA2_5HZ,
    INV_LPA2_10HZ,
    INV_LPA2_20HZ,
    INV_LPA2_40HZ,
    INV_LPA2_80HZ,
    INV_LPA2_160HZ,
    INV_LPA2_320HZ,
    INV_LPA2_640HZ
};

#define BIT_I2C_MST_VDDIO   (0x80)
#define BIT_FIFO_EN         (0x40)
#define BIT_DMP_EN          (0x80)
#define BIT_FIFO_RST        (0x04)
#define BIT_DMP_RST         (0x08)
#define BIT_FIFO_OVERFLOW   (0x10)
#define BIT_DATA_RDY_EN     (0x01)
#define BIT_DMP_INT_EN      (0x02)
#define BIT_MOT_INT_EN      (0x40)
#define BITS_FSR            (0x18)
#define BITS_LPF            (0x07)
#define BITS_HPF            (0x07)
#define BITS_CLK            (0x07)
#define BIT_FIFO_SIZE_1024  (0x40)
#define BIT_FIFO_SIZE_2048  (0x80)
#define BIT_FIFO_SIZE_4096  (0xC0)
#define BIT_RESET           (0x80)
#define BIT_SLEEP           (0x40)
#define BIT_S0_DELAY_EN     (0x01)
#define BIT_S2_DELAY_EN     (0x04)
#define BITS_SLAVE_LENGTH   (0x0F)
#define BIT_SLAVE_BYTE_SW   (0x40)
#define BIT_SLAVE_GROUP     (0x10)
#define BIT_SLAVE_EN        (0x80)
#define BIT_I2C_READ        (0x80)
#define BITS_I2C_MASTER_DLY (0x1F)
#define BIT_AUX_IF_EN       (0x20)
#define BIT_ACTL            (0x80)
#define BIT_LATCH_EN        (0x20)
#define BIT_ANY_RD_CLR      (0x10)
#define BIT_BYPASS_EN       (0x02)
#define BITS_WOM_EN         (0xC0)
#define BIT_LPA_CYCLE       (0x20)
#define BIT_STBY_XA         (0x20)
#define BIT_STBY_YA         (0x10)
#define BIT_STBY_ZA         (0x08)
#define BIT_STBY_XG         (0x04)
#define BIT_STBY_YG         (0x02)
#define BIT_STBY_ZG         (0x01)
#define BIT_STBY_XYZA       (BIT_STBY_XA | BIT_STBY_YA | BIT_STBY_ZA)
#define BIT_STBY_XYZG       (BIT_STBY_XG | BIT_STBY_YG | BIT_STBY_ZG)

// AK8975_SECONDARY
#define SUPPORTS_AK89xx_HIGH_SENS   (0x00)
#define AK89xx_FSR                  (9830)
// AK8963_SECONDARY
#define SUPPORTS_AK89xx_HIGH_SENS2   (0x10)
#define AK89xx_FSR2                  (4915)

// AK89xx_SECONDARY
#define AKM_REG_WHOAMI      (0x00)

#define AKM_REG_ST1         (0x02)
#define AKM_REG_HXL         (0x03)
#define AKM_REG_ST2         (0x09)

#define AKM_REG_CNTL        (0x0A)
#define AKM_REG_ASTC        (0x0C)
#define AKM_REG_ASAX        (0x10)
#define AKM_REG_ASAY        (0x11)
#define AKM_REG_ASAZ        (0x12)

#define AKM_DATA_READY      (0x01)
#define AKM_DATA_OVERRUN    (0x02)
#define AKM_OVERFLOW        (0x80)
#define AKM_DATA_ERROR      (0x40)

#define AKM_BIT_SELF_TEST   (0x40)

#define AKM_POWER_DOWN          (0x00 | SUPPORTS_AK89xx_HIGH_SENS)
#define AKM_SINGLE_MEASUREMENT  (0x01 | SUPPORTS_AK89xx_HIGH_SENS)
#define AKM_FUSE_ROM_ACCESS     (0x0F | SUPPORTS_AK89xx_HIGH_SENS)
#define AKM_MODE_SELF_TEST      (0x08 | SUPPORTS_AK89xx_HIGH_SENS)

#define AKM_POWER_DOWN2          (0x00 | SUPPORTS_AK89xx_HIGH_SENS2)
#define AKM_SINGLE_MEASUREMENT2  (0x01 | SUPPORTS_AK89xx_HIGH_SENS2)
#define AKM_FUSE_ROM_ACCESS2     (0x0F | SUPPORTS_AK89xx_HIGH_SENS2)
#define AKM_MODE_SELF_TEST2      (0x08 | SUPPORTS_AK89xx_HIGH_SENS2)

#define AKM_WHOAMI      (0x48)

#define INV_X_GYRO      (0x40)
#define INV_Y_GYRO      (0x20)
#define INV_Z_GYRO      (0x10)
#define INV_XYZ_GYRO    (INV_X_GYRO | INV_Y_GYRO | INV_Z_GYRO)
#define INV_XYZ_ACCEL   (0x08)
#define INV_XYZ_COMPASS (0x01)

// These defines are copied from dmpDefaultMPU6050.c in the general MPL
// releases. These defines may change for each DMP image, so be sure to modify
// these values when switching to a new image.
#define CFG_LP_QUAT             (2712)
#define END_ORIENT_TEMP         (1866)
#define CFG_27                  (2742)
#define CFG_20                  (2224)
#define CFG_23                  (2745)
#define CFG_FIFO_ON_EVENT       (2690)
#define END_PREDICTION_UPDATE   (1761)
#define CGNOTICE_INTR           (2620)
#define X_GRT_Y_TMP             (1358)
#define CFG_DR_INT              (1029)
#define CFG_AUTH                (1035)
#define UPDATE_PROP_ROT         (1835)
#define END_COMPARE_Y_X_TMP2    (1455)
#define SKIP_X_GRT_Y_TMP        (1359)
#define SKIP_END_COMPARE        (1435)
#define FCFG_3                  (1088)
#define FCFG_2                  (1066)
#define FCFG_1                  (1062)
#define END_COMPARE_Y_X_TMP3    (1434)
#define FCFG_7                  (1073)
#define FCFG_6                  (1106)
#define FLAT_STATE_END          (1713)
#define SWING_END_4             (1616)
#define SWING_END_2             (1565)
#define SWING_END_3             (1587)
#define SWING_END_1             (1550)
#define CFG_8                   (2718)
#define CFG_15                  (2727)
#define CFG_16                  (2746)
#define CFG_EXT_GYRO_BIAS       (1189)
#define END_COMPARE_Y_X_TMP     (1407)
#define DO_NOT_UPDATE_PROP_ROT  (1839)
#define CFG_7                   (1205)
#define FLAT_STATE_END_TEMP     (1683)
#define END_COMPARE_Y_X         (1484)
#define SKIP_SWING_END_1        (1551)
#define SKIP_SWING_END_3        (1588)
#define SKIP_SWING_END_2        (1566)
#define TILTG75_START           (1672)
#define CFG_6                   (2753)
#define TILTL75_END             (1669)
#define END_ORIENT              (1884)
#define CFG_FLICK_IN            (2573)
#define TILTL75_START           (1643)
#define CFG_MOTION_BIAS         (1208)
#define X_GRT_Y                 (1408)
#define TEMPLABEL               (2324)
#define CFG_ANDROID_ORIENT_INT  (1853)
#define CFG_GYRO_RAW_DATA       (2722)
#define X_GRT_Y_TMP2            (1379)

#define D_0_22                  (22+512)
#define D_0_24                  (24+512)

#define D_0_36                  (36)
#define D_0_52                  (52)
#define D_0_96                  (96)
#define D_0_104                 (104)
#define D_0_108                 (108)
#define D_0_163                 (163)
#define D_0_188                 (188)
#define D_0_192                 (192)
#define D_0_224                 (224)
#define D_0_228                 (228)
#define D_0_232                 (232)
#define D_0_236                 (236)

#define D_1_2                   (256 + 2)
#define D_1_4                   (256 + 4)
#define D_1_8                   (256 + 8)
#define D_1_10                  (256 + 10)
#define D_1_24                  (256 + 24)
#define D_1_28                  (256 + 28)
#define D_1_36                  (256 + 36)
#define D_1_40                  (256 + 40)
#define D_1_44                  (256 + 44)
#define D_1_72                  (256 + 72)
#define D_1_74                  (256 + 74)
#define D_1_79                  (256 + 79)
#define D_1_88                  (256 + 88)
#define D_1_90                  (256 + 90)
#define D_1_92                  (256 + 92)
#define D_1_96                  (256 + 96)
#define D_1_98                  (256 + 98)
#define D_1_106                 (256 + 106)
#define D_1_108                 (256 + 108)
#define D_1_112                 (256 + 112)
#define D_1_128                 (256 + 144)
#define D_1_152                 (256 + 12)
#define D_1_160                 (256 + 160)
#define D_1_176                 (256 + 176)
#define D_1_178                 (256 + 178)
#define D_1_218                 (256 + 218)
#define D_1_232                 (256 + 232)
#define D_1_236                 (256 + 236)
#define D_1_240                 (256 + 240)
#define D_1_244                 (256 + 244)
#define D_1_250                 (256 + 250)
#define D_1_252                 (256 + 252)
#define D_2_12                  (512 + 12)
#define D_2_96                  (512 + 96)
#define D_2_108                 (512 + 108)
#define D_2_208                 (512 + 208)
#define D_2_224                 (512 + 224)
#define D_2_236                 (512 + 236)
#define D_2_244                 (512 + 244)
#define D_2_248                 (512 + 248)
#define D_2_252                 (512 + 252)

#define CPASS_BIAS_X            (35 * 16 + 4)
#define CPASS_BIAS_Y            (35 * 16 + 8)
#define CPASS_BIAS_Z            (35 * 16 + 12)
#define CPASS_MTX_00            (36 * 16)
#define CPASS_MTX_01            (36 * 16 + 4)
#define CPASS_MTX_02            (36 * 16 + 8)
#define CPASS_MTX_10            (36 * 16 + 12)
#define CPASS_MTX_11            (37 * 16)
#define CPASS_MTX_12            (37 * 16 + 4)
#define CPASS_MTX_20            (37 * 16 + 8)
#define CPASS_MTX_21            (37 * 16 + 12)
#define CPASS_MTX_22            (43 * 16 + 12)
#define D_EXT_GYRO_BIAS_X       (61 * 16)
#define D_EXT_GYRO_BIAS_Y       (61 * 16) + 4
#define D_EXT_GYRO_BIAS_Z       (61 * 16) + 8
#define D_ACT0                  (40 * 16)
#define D_ACSX                  (40 * 16 + 4)
#define D_ACSY                  (40 * 16 + 8)
#define D_ACSZ                  (40 * 16 + 12)

#define FLICK_MSG               (45 * 16 + 4)
#define FLICK_COUNTER           (45 * 16 + 8)
#define FLICK_LOWER             (45 * 16 + 12)
#define FLICK_UPPER             (46 * 16 + 12)

#define D_AUTH_OUT              (992)
#define D_AUTH_IN               (996)
#define D_AUTH_A                (1000)
#define D_AUTH_B                (1004)

#define D_PEDSTD_BP_B           (768 + 0x1C)
#define D_PEDSTD_HP_A           (768 + 0x78)
#define D_PEDSTD_HP_B           (768 + 0x7C)
#define D_PEDSTD_BP_A4          (768 + 0x40)
#define D_PEDSTD_BP_A3          (768 + 0x44)
#define D_PEDSTD_BP_A2          (768 + 0x48)
#define D_PEDSTD_BP_A1          (768 + 0x4C)
#define D_PEDSTD_INT_THRSH      (768 + 0x68)
#define D_PEDSTD_CLIP           (768 + 0x6C)
#define D_PEDSTD_SB             (768 + 0x28)
#define D_PEDSTD_SB_TIME        (768 + 0x2C)
#define D_PEDSTD_PEAKTHRSH      (768 + 0x98)
#define D_PEDSTD_TIML           (768 + 0x2A)
#define D_PEDSTD_TIMH           (768 + 0x2E)
#define D_PEDSTD_PEAK           (768 + 0X94)
#define D_PEDSTD_STEPCTR        (768 + 0x60)
#define D_PEDSTD_TIMECTR        (964)
#define D_PEDSTD_DECI           (768 + 0xA0)

#define D_HOST_NO_MOT           (976)
#define D_ACCEL_BIAS            (660)

#define D_ORIENT_GAP            (76)

#define D_TILT0_H               (48)
#define D_TILT0_L               (50)
#define D_TILT1_H               (52)
#define D_TILT1_L               (54)
#define D_TILT2_H               (56)
#define D_TILT2_L               (58)
#define D_TILT3_H               (60)
#define D_TILT3_L               (62)

#define DMP_CODE_SIZE           (3062)

#ifdef __SAM3X8E__
const uint8_t dmp_memory[DMP_CODE_SIZE] = {
#else
const unsigned char dmp_memory[DMP_CODE_SIZE] PROGMEM = {
#endif
    /* bank # 0 */
    0x00, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x02, 0x00, 0x03, 0x00, 0x00,
    0x00, 0x65, 0x00, 0x54, 0xff, 0xef, 0x00, 0x00, 0xfa, 0x80, 0x00, 0x0b, 0x12, 0x82, 0x00, 0x01,
    0x03, 0x0c, 0x30, 0xc3, 0x0e, 0x8c, 0x8c, 0xe9, 0x14, 0xd5, 0x40, 0x02, 0x13, 0x71, 0x0f, 0x8e,
    0x38, 0x83, 0xf8, 0x83, 0x30, 0x00, 0xf8, 0x83, 0x25, 0x8e, 0xf8, 0x83, 0x30, 0x00, 0xf8, 0x83,
    0xff, 0xff, 0xff, 0xff, 0x0f, 0xfe, 0xa9, 0xd6, 0x24, 0x00, 0x04, 0x00, 0x1a, 0x82, 0x79, 0xa1,
    0x00, 0x00, 0x00, 0x3c, 0xff, 0xff, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x38, 0x83, 0x6f, 0xa2,
    0x00, 0x3e, 0x03, 0x30, 0x40, 0x00, 0x00, 0x00, 0x02, 0xca, 0xe3, 0x09, 0x3e, 0x80, 0x00, 0x00,
    0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x60, 0x00, 0x00, 0x00,
    0x00, 0x0c, 0x00, 0x00, 0x00, 0x0c, 0x18, 0x6e, 0x00, 0x00, 0x06, 0x92, 0x0a, 0x16, 0xc0, 0xdf,
    0xff, 0xff, 0x02, 0x56, 0xfd, 0x8c, 0xd3, 0x77, 0xff, 0xe1, 0xc4, 0x96, 0xe0, 0xc5, 0xbe, 0xaa,
    0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x0b, 0x2b, 0x00, 0x00, 0x16, 0x57, 0x00, 0x00, 0x03, 0x59,
    0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1d, 0xfa, 0x00, 0x02, 0x6c, 0x1d, 0x00, 0x00, 0x00, 0x00,
    0x3f, 0xff, 0xdf, 0xeb, 0x00, 0x3e, 0xb3, 0xb6, 0x00, 0x0d, 0x22, 0x78, 0x00, 0x00, 0x2f, 0x3c,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x19, 0x42, 0xb5, 0x00, 0x00, 0x39, 0xa2, 0x00, 0x00, 0xb3, 0x65,
    0xd9, 0x0e, 0x9f, 0xc9, 0x1d, 0xcf, 0x4c, 0x34, 0x30, 0x00, 0x00, 0x00, 0x50, 0x00, 0x00, 0x00,
    0x3b, 0xb6, 0x7a, 0xe8, 0x00, 0x64, 0x00, 0x00, 0x00, 0xc8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    /* bank # 1 */
    0x10, 0x00, 0x00, 0x00, 0x10, 0x00, 0xfa, 0x92, 0x10, 0x00, 0x22, 0x5e, 0x00, 0x0d, 0x22, 0x9f,
    0x00, 0x01, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0xff, 0x46, 0x00, 0x00, 0x63, 0xd4, 0x00, 0x00,
    0x10, 0x00, 0x00, 0x00, 0x04, 0xd6, 0x00, 0x00, 0x04, 0xcc, 0x00, 0x00, 0x04, 0xcc, 0x00, 0x00,
    0x00, 0x00, 0x10, 0x72, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x06, 0x00, 0x02, 0x00, 0x05, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x05, 0x00, 0x64, 0x00, 0x20, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x03, 0x00,
    0x00, 0x00, 0x00, 0x32, 0xf8, 0x98, 0x00, 0x00, 0xff, 0x65, 0x00, 0x00, 0x83, 0x0f, 0x00, 0x00,
    0xff, 0x9b, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00,
    0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0xb2, 0x6a, 0x00, 0x02, 0x00, 0x00,
    0x00, 0x01, 0xfb, 0x83, 0x00, 0x68, 0x00, 0x00, 0x00, 0xd9, 0xfc, 0x00, 0x7c, 0xf1, 0xff, 0x83,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00, 0x64, 0x03, 0xe8, 0x00, 0x64, 0x00, 0x28,
    0x00, 0x00, 0x00, 0x25, 0x00, 0x00, 0x00, 0x00, 0x16, 0xa0, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00,
    0x00, 0x00, 0x10, 0x00, 0x00, 0x2f, 0x00, 0x00, 0x00, 0x00, 0x01, 0xf4, 0x00, 0x00, 0x10, 0x00,
    /* bank # 2 */
    0x00, 0x28, 0x00, 0x00, 0xff, 0xff, 0x45, 0x81, 0xff, 0xff, 0xfa, 0x72, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x44, 0x00, 0x05, 0x00, 0x05, 0xba, 0xc6, 0x00, 0x47, 0x78, 0xa2,
    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x14,
    0x00, 0x00, 0x25, 0x4d, 0x00, 0x2f, 0x70, 0x6d, 0x00, 0x00, 0x05, 0xae, 0x00, 0x0c, 0x02, 0xd0,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x1b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x64, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x1b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0e, 0x00, 0x0e,
    0x00, 0x00, 0x0a, 0xc7, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32, 0xff, 0xff, 0xff, 0x9c,
    0x00, 0x00, 0x0b, 0x2b, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x64,
    0xff, 0xe5, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    /* bank # 3 */
    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x01, 0x80, 0x00, 0x00, 0x01, 0x80, 0x00, 0x00, 0x01, 0x80, 0x00, 0x00, 0x24, 0x26, 0xd3,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x10, 0x00, 0x96, 0x00, 0x3c,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x0c, 0x0a, 0x4e, 0x68, 0xcd, 0xcf, 0x77, 0x09, 0x50, 0x16, 0x67, 0x59, 0xc6, 0x19, 0xce, 0x82,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0xd7, 0x84, 0x00, 0x03, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc7, 0x93, 0x8f, 0x9d, 0x1e, 0x1b, 0x1c, 0x19,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x18, 0x85, 0x00, 0x00, 0x40, 0x00,
    0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x67, 0x7d, 0xdf, 0x7e, 0x72, 0x90, 0x2e, 0x55, 0x4c, 0xf6, 0xe6, 0x88,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,

    /* bank # 4 */
    0xd8, 0xdc, 0xb4, 0xb8, 0xb0, 0xd8, 0xb9, 0xab, 0xf3, 0xf8, 0xfa, 0xb3, 0xb7, 0xbb, 0x8e, 0x9e,
    0xae, 0xf1, 0x32, 0xf5, 0x1b, 0xf1, 0xb4, 0xb8, 0xb0, 0x80, 0x97, 0xf1, 0xa9, 0xdf, 0xdf, 0xdf,
    0xaa, 0xdf, 0xdf, 0xdf, 0xf2, 0xaa, 0xc5, 0xcd, 0xc7, 0xa9, 0x0c, 0xc9, 0x2c, 0x97, 0xf1, 0xa9,
    0x89, 0x26, 0x46, 0x66, 0xb2, 0x89, 0x99, 0xa9, 0x2d, 0x55, 0x7d, 0xb0, 0xb0, 0x8a, 0xa8, 0x96,
    0x36, 0x56, 0x76, 0xf1, 0xba, 0xa3, 0xb4, 0xb2, 0x80, 0xc0, 0xb8, 0xa8, 0x97, 0x11, 0xb2, 0x83,
    0x98, 0xba, 0xa3, 0xf0, 0x24, 0x08, 0x44, 0x10, 0x64, 0x18, 0xb2, 0xb9, 0xb4, 0x98, 0x83, 0xf1,
    0xa3, 0x29, 0x55, 0x7d, 0xba, 0xb5, 0xb1, 0xa3, 0x83, 0x93, 0xf0, 0x00, 0x28, 0x50, 0xf5, 0xb2,
    0xb6, 0xaa, 0x83, 0x93, 0x28, 0x54, 0x7c, 0xf1, 0xb9, 0xa3, 0x82, 0x93, 0x61, 0xba, 0xa2, 0xda,
    0xde, 0xdf, 0xdb, 0x81, 0x9a, 0xb9, 0xae, 0xf5, 0x60, 0x68, 0x70, 0xf1, 0xda, 0xba, 0xa2, 0xdf,
    0xd9, 0xba, 0xa2, 0xfa, 0xb9, 0xa3, 0x82, 0x92, 0xdb, 0x31, 0xba, 0xa2, 0xd9, 0xba, 0xa2, 0xf8,
    0xdf, 0x85, 0xa4, 0xd0, 0xc1, 0xbb, 0xad, 0x83, 0xc2, 0xc5, 0xc7, 0xb8, 0xa2, 0xdf, 0xdf, 0xdf,
    0xba, 0xa0, 0xdf, 0xdf, 0xdf, 0xd8, 0xd8, 0xf1, 0xb8, 0xaa, 0xb3, 0x8d, 0xb4, 0x98, 0x0d, 0x35,
    0x5d, 0xb2, 0xb6, 0xba, 0xaf, 0x8c, 0x96, 0x19, 0x8f, 0x9f, 0xa7, 0x0e, 0x16, 0x1e, 0xb4, 0x9a,
    0xb8, 0xaa, 0x87, 0x2c, 0x54, 0x7c, 0xba, 0xa4, 0xb0, 0x8a, 0xb6, 0x91, 0x32, 0x56, 0x76, 0xb2,
    0x84, 0x94, 0xa4, 0xc8, 0x08, 0xcd, 0xd8, 0xb8, 0xb4, 0xb0, 0xf1, 0x99, 0x82, 0xa8, 0x2d, 0x55,
    0x7d, 0x98, 0xa8, 0x0e, 0x16, 0x1e, 0xa2, 0x2c, 0x54, 0x7c, 0x92, 0xa4, 0xf0, 0x2c, 0x50, 0x78,
    /* bank # 5 */
    0xf1, 0x84, 0xa8, 0x98, 0xc4, 0xcd, 0xfc, 0xd8, 0x0d, 0xdb, 0xa8, 0xfc, 0x2d, 0xf3, 0xd9, 0xba,
    0xa6, 0xf8, 0xda, 0xba, 0xa6, 0xde, 0xd8, 0xba, 0xb2, 0xb6, 0x86, 0x96, 0xa6, 0xd0, 0xf3, 0xc8,
    0x41, 0xda, 0xa6, 0xc8, 0xf8, 0xd8, 0xb0, 0xb4, 0xb8, 0x82, 0xa8, 0x92, 0xf5, 0x2c, 0x54, 0x88,
    0x98, 0xf1, 0x35, 0xd9, 0xf4, 0x18, 0xd8, 0xf1, 0xa2, 0xd0, 0xf8, 0xf9, 0xa8, 0x84, 0xd9, 0xc7,
    0xdf, 0xf8, 0xf8, 0x83, 0xc5, 0xda, 0xdf, 0x69, 0xdf, 0x83, 0xc1, 0xd8, 0xf4, 0x01, 0x14, 0xf1,
    0xa8, 0x82, 0x4e, 0xa8, 0x84, 0xf3, 0x11, 0xd1, 0x82, 0xf5, 0xd9, 0x92, 0x28, 0x97, 0x88, 0xf1,
    0x09, 0xf4, 0x1c, 0x1c, 0xd8, 0x84, 0xa8, 0xf3, 0xc0, 0xf9, 0xd1, 0xd9, 0x97, 0x82, 0xf1, 0x29,
    0xf4, 0x0d, 0xd8, 0xf3, 0xf9, 0xf9, 0xd1, 0xd9, 0x82, 0xf4, 0xc2, 0x03, 0xd8, 0xde, 0xdf, 0x1a,
    0xd8, 0xf1, 0xa2, 0xfa, 0xf9, 0xa8, 0x84, 0x98, 0xd9, 0xc7, 0xdf, 0xf8, 0xf8, 0xf8, 0x83, 0xc7,
    0xda, 0xdf, 0x69, 0xdf, 0xf8, 0x83, 0xc3, 0xd8, 0xf4, 0x01, 0x14, 0xf1, 0x98, 0xa8, 0x82, 0x2e,
    0xa8, 0x84, 0xf3, 0x11, 0xd1, 0x82, 0xf5, 0xd9, 0x92, 0x50, 0x97, 0x88, 0xf1, 0x09, 0xf4, 0x1c,
    0xd8, 0x84, 0xa8, 0xf3, 0xc0, 0xf8, 0xf9, 0xd1, 0xd9, 0x97, 0x82, 0xf1, 0x49, 0xf4, 0x0d, 0xd8,
    0xf3, 0xf9, 0xf9, 0xd1, 0xd9, 0x82, 0xf4, 0xc4, 0x03, 0xd8, 0xde, 0xdf, 0xd8, 0xf1, 0xad, 0x88,
    0x98, 0xcc, 0xa8, 0x09, 0xf9, 0xd9, 0x82, 0x92, 0xa8, 0xf5, 0x7c, 0xf1, 0x88, 0x3a, 0xcf, 0x94,
    0x4a, 0x6e, 0x98, 0xdb, 0x69, 0x31, 0xda, 0xad, 0xf2, 0xde, 0xf9, 0xd8, 0x87, 0x95, 0xa8, 0xf2,
    0x21, 0xd1, 0xda, 0xa5, 0xf9, 0xf4, 0x17, 0xd9, 0xf1, 0xae, 0x8e, 0xd0, 0xc0, 0xc3, 0xae, 0x82,
    /* bank # 6 */
    0xc6, 0x84, 0xc3, 0xa8, 0x85, 0x95, 0xc8, 0xa5, 0x88, 0xf2, 0xc0, 0xf1, 0xf4, 0x01, 0x0e, 0xf1,
    0x8e, 0x9e, 0xa8, 0xc6, 0x3e, 0x56, 0xf5, 0x54, 0xf1, 0x88, 0x72, 0xf4, 0x01, 0x15, 0xf1, 0x98,
    0x45, 0x85, 0x6e, 0xf5, 0x8e, 0x9e, 0x04, 0x88, 0xf1, 0x42, 0x98, 0x5a, 0x8e, 0x9e, 0x06, 0x88,
    0x69, 0xf4, 0x01, 0x1c, 0xf1, 0x98, 0x1e, 0x11, 0x08, 0xd0, 0xf5, 0x04, 0xf1, 0x1e, 0x97, 0x02,
    0x02, 0x98, 0x36, 0x25, 0xdb, 0xf9, 0xd9, 0x85, 0xa5, 0xf3, 0xc1, 0xda, 0x85, 0xa5, 0xf3, 0xdf,
    0xd8, 0x85, 0x95, 0xa8, 0xf3, 0x09, 0xda, 0xa5, 0xfa, 0xd8, 0x82, 0x92, 0xa8, 0xf5, 0x78, 0xf1,
    0x88, 0x1a, 0x84, 0x9f, 0x26, 0x88, 0x98, 0x21, 0xda, 0xf4, 0x1d, 0xf3, 0xd8, 0x87, 0x9f, 0x39,
    0xd1, 0xaf, 0xd9, 0xdf, 0xdf, 0xfb, 0xf9, 0xf4, 0x0c, 0xf3, 0xd8, 0xfa, 0xd0, 0xf8, 0xda, 0xf9,
    0xf9, 0xd0, 0xdf, 0xd9, 0xf9, 0xd8, 0xf4, 0x0b, 0xd8, 0xf3, 0x87, 0x9f, 0x39, 0xd1, 0xaf, 0xd9,
    0xdf, 0xdf, 0xf4, 0x1d, 0xf3, 0xd8, 0xfa, 0xfc, 0xa8, 0x69, 0xf9, 0xf9, 0xaf, 0xd0, 0xda, 0xde,
    0xfa, 0xd9, 0xf8, 0x8f, 0x9f, 0xa8, 0xf1, 0xcc, 0xf3, 0x98, 0xdb, 0x45, 0xd9, 0xaf, 0xdf, 0xd0,
    0xf8, 0xd8, 0xf1, 0x8f, 0x9f, 0xa8, 0xca, 0xf3, 0x88, 0x09, 0xda, 0xaf, 0x8f, 0xcb, 0xf8, 0xd8,
    0xf2, 0xad, 0x97, 0x8d, 0x0c, 0xd9, 0xa5, 0xdf, 0xf9, 0xba, 0xa6, 0xf3, 0xfa, 0xf4, 0x12, 0xf2,
    0xd8, 0x95, 0x0d, 0xd1, 0xd9, 0xba, 0xa6, 0xf3, 0xfa, 0xda, 0xa5, 0xf2, 0xc1, 0xba, 0xa6, 0xf3,
    0xdf, 0xd8, 0xf1, 0xba, 0xb2, 0xb6, 0x86, 0x96, 0xa6, 0xd0, 0xca, 0xf3, 0x49, 0xda, 0xa6, 0xcb,
    0xf8, 0xd8, 0xb0, 0xb4, 0xb8, 0xd8, 0xad, 0x84, 0xf2, 0xc0, 0xdf, 0xf1, 0x8f, 0xcb, 0xc3, 0xa8,
    /* bank # 7 */
    0xb2, 0xb6, 0x86, 0x96, 0xc8, 0xc1, 0xcb, 0xc3, 0xf3, 0xb0, 0xb4, 0x88, 0x98, 0xa8, 0x21, 0xdb,
    0x71, 0x8d, 0x9d, 0x71, 0x85, 0x95, 0x21, 0xd9, 0xad, 0xf2, 0xfa, 0xd8, 0x85, 0x97, 0xa8, 0x28,
    0xd9, 0xf4, 0x08, 0xd8, 0xf2, 0x8d, 0x29, 0xda, 0xf4, 0x05, 0xd9, 0xf2, 0x85, 0xa4, 0xc2, 0xf2,
    0xd8, 0xa8, 0x8d, 0x94, 0x01, 0xd1, 0xd9, 0xf4, 0x11, 0xf2, 0xd8, 0x87, 0x21, 0xd8, 0xf4, 0x0a,
    0xd8, 0xf2, 0x84, 0x98, 0xa8, 0xc8, 0x01, 0xd1, 0xd9, 0xf4, 0x11, 0xd8, 0xf3, 0xa4, 0xc8, 0xbb,
    0xaf, 0xd0, 0xf2, 0xde, 0xf8, 0xf8, 0xf8, 0xf8, 0xf8, 0xf8, 0xf8, 0xf8, 0xd8, 0xf1, 0xb8, 0xf6,
    0xb5, 0xb9, 0xb0, 0x8a, 0x95, 0xa3, 0xde, 0x3c, 0xa3, 0xd9, 0xf8, 0xd8, 0x5c, 0xa3, 0xd9, 0xf8,
    0xd8, 0x7c, 0xa3, 0xd9, 0xf8, 0xd8, 0xf8, 0xf9, 0xd1, 0xa5, 0xd9, 0xdf, 0xda, 0xfa, 0xd8, 0xb1,
    0x85, 0x30, 0xf7, 0xd9, 0xde, 0xd8, 0xf8, 0x30, 0xad, 0xda, 0xde, 0xd8, 0xf2, 0xb4, 0x8c, 0x99,
    0xa3, 0x2d, 0x55, 0x7d, 0xa0, 0x83, 0xdf, 0xdf, 0xdf, 0xb5, 0x91, 0xa0, 0xf6, 0x29, 0xd9, 0xfb,
    0xd8, 0xa0, 0xfc, 0x29, 0xd9, 0xfa, 0xd8, 0xa0, 0xd0, 0x51, 0xd9, 0xf8, 0xd8, 0xfc, 0x51, 0xd9,
    0xf9, 0xd8, 0x79, 0xd9, 0xfb, 0xd8, 0xa0, 0xd0, 0xfc, 0x79, 0xd9, 0xfa, 0xd8, 0xa1, 0xf9, 0xf9,
    0xf9, 0xf9, 0xf9, 0xa0, 0xda, 0xdf, 0xdf, 0xdf, 0xd8, 0xa1, 0xf8, 0xf8, 0xf8, 0xf8, 0xf8, 0xac,
    0xde, 0xf8, 0xad, 0xde, 0x83, 0x93, 0xac, 0x2c, 0x54, 0x7c, 0xf1, 0xa8, 0xdf, 0xdf, 0xdf, 0xf6,
    0x9d, 0x2c, 0xda, 0xa0, 0xdf, 0xd9, 0xfa, 0xdb, 0x2d, 0xf8, 0xd8, 0xa8, 0x50, 0xda, 0xa0, 0xd0,
    0xde, 0xd9, 0xd0, 0xf8, 0xf8, 0xf8, 0xdb, 0x55, 0xf8, 0xd8, 0xa8, 0x78, 0xda, 0xa0, 0xd0, 0xdf,
    /* bank # 8 */
    0xd9, 0xd0, 0xfa, 0xf8, 0xf8, 0xf8, 0xf8, 0xdb, 0x7d, 0xf8, 0xd8, 0x9c, 0xa8, 0x8c, 0xf5, 0x30,
    0xdb, 0x38, 0xd9, 0xd0, 0xde, 0xdf, 0xa0, 0xd0, 0xde, 0xdf, 0xd8, 0xa8, 0x48, 0xdb, 0x58, 0xd9,
    0xdf, 0xd0, 0xde, 0xa0, 0xdf, 0xd0, 0xde, 0xd8, 0xa8, 0x68, 0xdb, 0x70, 0xd9, 0xdf, 0xdf, 0xa0,
    0xdf, 0xdf, 0xd8, 0xf1, 0xa8, 0x88, 0x90, 0x2c, 0x54, 0x7c, 0x98, 0xa8, 0xd0, 0x5c, 0x38, 0xd1,
    0xda, 0xf2, 0xae, 0x8c, 0xdf, 0xf9, 0xd8, 0xb0, 0x87, 0xa8, 0xc1, 0xc1, 0xb1, 0x88, 0xa8, 0xc6,
    0xf9, 0xf9, 0xda, 0x36, 0xd8, 0xa8, 0xf9, 0xda, 0x36, 0xd8, 0xa8, 0xf9, 0xda, 0x36, 0xd8, 0xa8,
    0xf9, 0xda, 0x36, 0xd8, 0xa8, 0xf9, 0xda, 0x36, 0xd8, 0xf7, 0x8d, 0x9d, 0xad, 0xf8, 0x18, 0xda,
    0xf2, 0xae, 0xdf, 0xd8, 0xf7, 0xad, 0xfa, 0x30, 0xd9, 0xa4, 0xde, 0xf9, 0xd8, 0xf2, 0xae, 0xde,
    0xfa, 0xf9, 0x83, 0xa7, 0xd9, 0xc3, 0xc5, 0xc7, 0xf1, 0x88, 0x9b, 0xa7, 0x7a, 0xad, 0xf7, 0xde,
    0xdf, 0xa4, 0xf8, 0x84, 0x94, 0x08, 0xa7, 0x97, 0xf3, 0x00, 0xae, 0xf2, 0x98, 0x19, 0xa4, 0x88,
    0xc6, 0xa3, 0x94, 0x88, 0xf6, 0x32, 0xdf, 0xf2, 0x83, 0x93, 0xdb, 0x09, 0xd9, 0xf2, 0xaa, 0xdf,
    0xd8, 0xd8, 0xae, 0xf8, 0xf9, 0xd1, 0xda, 0xf3, 0xa4, 0xde, 0xa7, 0xf1, 0x88, 0x9b, 0x7a, 0xd8,
    0xf3, 0x84, 0x94, 0xae, 0x19, 0xf9, 0xda, 0xaa, 0xf1, 0xdf, 0xd8, 0xa8, 0x81, 0xc0, 0xc3, 0xc5,
    0xc7, 0xa3, 0x92, 0x83, 0xf6, 0x28, 0xad, 0xde, 0xd9, 0xf8, 0xd8, 0xa3, 0x50, 0xad, 0xd9, 0xf8,
    0xd8, 0xa3, 0x78, 0xad, 0xd9, 0xf8, 0xd8, 0xf8, 0xf9, 0xd1, 0xa1, 0xda, 0xde, 0xc3, 0xc5, 0xc7,
    0xd8, 0xa1, 0x81, 0x94, 0xf8, 0x18, 0xf2, 0xb0, 0x89, 0xac, 0xc3, 0xc5, 0xc7, 0xf1, 0xd8, 0xb8,
    /* bank # 9 */
    0xb4, 0xb0, 0x97, 0x86, 0xa8, 0x31, 0x9b, 0x06, 0x99, 0x07, 0xab, 0x97, 0x28, 0x88, 0x9b, 0xf0,
    0x0c, 0x20, 0x14, 0x40, 0xb0, 0xb4, 0xb8, 0xf0, 0xa8, 0x8a, 0x9a, 0x28, 0x50, 0x78, 0xb7, 0x9b,
    0xa8, 0x29, 0x51, 0x79, 0x24, 0x70, 0x59, 0x44, 0x69, 0x38, 0x64, 0x48, 0x31, 0xf1, 0xbb, 0xab,
    0x88, 0x00, 0x2c, 0x54, 0x7c, 0xf0, 0xb3, 0x8b, 0xb8, 0xa8, 0x04, 0x28, 0x50, 0x78, 0xf1, 0xb0,
    0x88, 0xb4, 0x97, 0x26, 0xa8, 0x59, 0x98, 0xbb, 0xab, 0xb3, 0x8b, 0x02, 0x26, 0x46, 0x66, 0xb0,
    0xb8, 0xf0, 0x8a, 0x9c, 0xa8, 0x29, 0x51, 0x79, 0x8b, 0x29, 0x51, 0x79, 0x8a, 0x24, 0x70, 0x59,
    0x8b, 0x20, 0x58, 0x71, 0x8a, 0x44, 0x69, 0x38, 0x8b, 0x39, 0x40, 0x68, 0x8a, 0x64, 0x48, 0x31,
    0x8b, 0x30, 0x49, 0x60, 0x88, 0xf1, 0xac, 0x00, 0x2c, 0x54, 0x7c, 0xf0, 0x8c, 0xa8, 0x04, 0x28,
    0x50, 0x78, 0xf1, 0x88, 0x97, 0x26, 0xa8, 0x59, 0x98, 0xac, 0x8c, 0x02, 0x26, 0x46, 0x66, 0xf0,
    0x89, 0x9c, 0xa8, 0x29, 0x51, 0x79, 0x24, 0x70, 0x59, 0x44, 0x69, 0x38, 0x64, 0x48, 0x31, 0xa9,
    0x88, 0x09, 0x20, 0x59, 0x70, 0xab, 0x11, 0x38, 0x40, 0x69, 0xa8, 0x19, 0x31, 0x48, 0x60, 0x8c,
    0xa8, 0x3c, 0x41, 0x5c, 0x20, 0x7c, 0x00, 0xf1, 0x87, 0x98, 0x19, 0x86, 0xa8, 0x6e, 0x76, 0x7e,
    0xa9, 0x99, 0x88, 0x2d, 0x55, 0x7d, 0xd8, 0xb1, 0xb5, 0xb9, 0xa3, 0xdf, 0xdf, 0xdf, 0xae, 0xd0,
    0xdf, 0xaa, 0xd0, 0xde, 0xf2, 0xab, 0xf8, 0xf9, 0xd9, 0xb0, 0x87, 0xc4, 0xaa, 0xf1, 0xdf, 0xdf,
    0xbb, 0xaf, 0xdf, 0xdf, 0xb9, 0xd8, 0xb1, 0xf1, 0xa3, 0x97, 0x8e, 0x60, 0xdf, 0xb0, 0x84, 0xf2,
    0xc8, 0xf8, 0xf9, 0xd9, 0xde, 0xd8, 0x93, 0x85, 0xf1, 0x4a, 0xb1, 0x83, 0xa3, 0x08, 0xb5, 0x83,
    /* bank # 10 */
    0x9a, 0x08, 0x10, 0xb7, 0x9f, 0x10, 0xd8, 0xf1, 0xb0, 0xba, 0xae, 0xb0, 0x8a, 0xc2, 0xb2, 0xb6,
    0x8e, 0x9e, 0xf1, 0xfb, 0xd9, 0xf4, 0x1d, 0xd8, 0xf9, 0xd9, 0x0c, 0xf1, 0xd8, 0xf8, 0xf8, 0xad,
    0x61, 0xd9, 0xae, 0xfb, 0xd8, 0xf4, 0x0c, 0xf1, 0xd8, 0xf8, 0xf8, 0xad, 0x19, 0xd9, 0xae, 0xfb,
    0xdf, 0xd8, 0xf4, 0x16, 0xf1, 0xd8, 0xf8, 0xad, 0x8d, 0x61, 0xd9, 0xf4, 0xf4, 0xac, 0xf5, 0x9c,
    0x9c, 0x8d, 0xdf, 0x2b, 0xba, 0xb6, 0xae, 0xfa, 0xf8, 0xf4, 0x0b, 0xd8, 0xf1, 0xae, 0xd0, 0xf8,
    0xad, 0x51, 0xda, 0xae, 0xfa, 0xf8, 0xf1, 0xd8, 0xb9, 0xb1, 0xb6, 0xa3, 0x83, 0x9c, 0x08, 0xb9,
    0xb1, 0x83, 0x9a, 0xb5, 0xaa, 0xc0, 0xfd, 0x30, 0x83, 0xb7, 0x9f, 0x10, 0xb5, 0x8b, 0x93, 0xf2,
    0x02, 0x02, 0xd1, 0xab, 0xda, 0xde, 0xd8, 0xf1, 0xb0, 0x80, 0xba, 0xab, 0xc0, 0xc3, 0xb2, 0x84,
    0xc1, 0xc3, 0xd8, 0xb1, 0xb9, 0xf3, 0x8b, 0xa3, 0x91, 0xb6, 0x09, 0xb4, 0xd9, 0xab, 0xde, 0xb0,
    0x87, 0x9c, 0xb9, 0xa3, 0xdd, 0xf1, 0xb3, 0x8b, 0x8b, 0x8b, 0x8b, 0x8b, 0xb0, 0x87, 0xa3, 0xa3,
    0xa3, 0xa3, 0xb2, 0x8b, 0xb6, 0x9b, 0xf2, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3,
    0xa3, 0xf1, 0xb0, 0x87, 0xb5, 0x9a, 0xa3, 0xf3, 0x9b, 0xa3, 0xa3, 0xdc, 0xba, 0xac, 0xdf, 0xb9,
    0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3, 0xa3,
    0xd8, 0xd8, 0xd8, 0xbb, 0xb3, 0xb7, 0xf1, 0xaa, 0xf9, 0xda, 0xff, 0xd9, 0x80, 0x9a, 0xaa, 0x28,
    0xb4, 0x80, 0x98, 0xa7, 0x20, 0xb7, 0x97, 0x87, 0xa8, 0x66, 0x88, 0xf0, 0x79, 0x51, 0xf1, 0x90,
    0x2c, 0x87, 0x0c, 0xa7, 0x81, 0x97, 0x62, 0x93, 0xf0, 0x71, 0x71, 0x60, 0x85, 0x94, 0x01, 0x29,
    /* bank # 11 */
    0x51, 0x79, 0x90, 0xa5, 0xf1, 0x28, 0x4c, 0x6c, 0x87, 0x0c, 0x95, 0x18, 0x85, 0x78, 0xa3, 0x83,
    0x90, 0x28, 0x4c, 0x6c, 0x88, 0x6c, 0xd8, 0xf3, 0xa2, 0x82, 0x00, 0xf2, 0x10, 0xa8, 0x92, 0x19,
    0x80, 0xa2, 0xf2, 0xd9, 0x26, 0xd8, 0xf1, 0x88, 0xa8, 0x4d, 0xd9, 0x48, 0xd8, 0x96, 0xa8, 0x39,
    0x80, 0xd9, 0x3c, 0xd8, 0x95, 0x80, 0xa8, 0x39, 0xa6, 0x86, 0x98, 0xd9, 0x2c, 0xda, 0x87, 0xa7,
    0x2c, 0xd8, 0xa8, 0x89, 0x95, 0x19, 0xa9, 0x80, 0xd9, 0x38, 0xd8, 0xa8, 0x89, 0x39, 0xa9, 0x80,
    0xda, 0x3c, 0xd8, 0xa8, 0x2e, 0xa8, 0x39, 0x90, 0xd9, 0x0c, 0xd8, 0xa8, 0x95, 0x31, 0x98, 0xd9,
    0x0c, 0xd8, 0xa8, 0x09, 0xd9, 0xff, 0xd8, 0x01, 0xda, 0xff, 0xd8, 0x95, 0x39, 0xa9, 0xda, 0x26,
    0xff, 0xd8, 0x90, 0xa8, 0x0d, 0x89, 0x99, 0xa8, 0x10, 0x80, 0x98, 0x21, 0xda, 0x2e, 0xd8, 0x89,
    0x99, 0xa8, 0x31, 0x80, 0xda, 0x2e, 0xd8, 0xa8, 0x86, 0x96, 0x31, 0x80, 0xda, 0x2e, 0xd8, 0xa8,
    0x87, 0x31, 0x80, 0xda, 0x2e, 0xd8, 0xa8, 0x82, 0x92, 0xf3, 0x41, 0x80, 0xf1, 0xd9, 0x2e, 0xd8,
    0xa8, 0x82, 0xf3, 0x19, 0x80, 0xf1, 0xd9, 0x2e, 0xd8, 0x82, 0xac, 0xf3, 0xc0, 0xa2, 0x80, 0x22,
    0xf1, 0xa6, 0x2e, 0xa7, 0x2e, 0xa9, 0x22, 0x98, 0xa8, 0x29, 0xda, 0xac, 0xde, 0xff, 0xd8, 0xa2,
    0xf2, 0x2a, 0xf1, 0xa9, 0x2e, 0x82, 0x92, 0xa8, 0xf2, 0x31, 0x80, 0xa6, 0x96, 0xf1, 0xd9, 0x00,
    0xac, 0x8c, 0x9c, 0x0c, 0x30, 0xac, 0xde, 0xd0, 0xde, 0xff, 0xd8, 0x8c, 0x9c, 0xac, 0xd0, 0x10,
    0xac, 0xde, 0x80, 0x92, 0xa2, 0xf2, 0x4c, 0x82, 0xa8, 0xf1, 0xca, 0xf2, 0x35, 0xf1, 0x96, 0x88,
    0xa6, 0xd9, 0x00, 0xd8, 0xf1, 0xff
};

static const unsigned short sStartAddress = 0x0400;

#define TAP_X               (0x01)
#define TAP_Y               (0x02)
#define TAP_Z               (0x04)
#define TAP_XYZ             (0x07)

#define TAP_X_UP            (0x01)
#define TAP_X_DOWN          (0x02)
#define TAP_Y_UP            (0x03)
#define TAP_Y_DOWN          (0x04)
#define TAP_Z_UP            (0x05)
#define TAP_Z_DOWN          (0x06)

#define ANDROID_ORIENT_PORTRAIT             (0x00)
#define ANDROID_ORIENT_LANDSCAPE            (0x01)
#define ANDROID_ORIENT_REVERSE_PORTRAIT     (0x02)
#define ANDROID_ORIENT_REVERSE_LANDSCAPE    (0x03)

#define DMP_INT_GESTURE     (0x01)
#define DMP_INT_CONTINUOUS  (0x02)

#define DMP_FEATURE_TAP             (0x001)
#define DMP_FEATURE_ANDROID_ORIENT  (0x002)
#define DMP_FEATURE_LP_QUAT         (0x004)
#define DMP_FEATURE_PEDOMETER       (0x008)
#define DMP_FEATURE_6X_LP_QUAT      (0x010)
#define DMP_FEATURE_GYRO_CAL        (0x020)
#define DMP_FEATURE_SEND_RAW_ACCEL  (0x040)
#define DMP_FEATURE_SEND_RAW_GYRO   (0x080)
#define DMP_FEATURE_SEND_CAL_GYRO   (0x100)

#define INV_WXYZ_QUAT       (0x100)

#define DMP_PTAT    0
#define DMP_XGYR    2
#define DMP_YGYR    4
#define DMP_ZGYR    6
#define DMP_XACC    8
#define DMP_YACC    10
#define DMP_ZACC    12
#define DMP_ADC1    14
#define DMP_ADC2    16
#define DMP_ADC3    18
#define DMP_BIASUNC    20
#define DMP_FIFORT    22
#define DMP_INVGSFH    24
#define DMP_INVGSFL    26
#define DMP_1H    28
#define DMP_1L    30
#define DMP_BLPFSTCH    32
#define DMP_BLPFSTCL    34
#define DMP_BLPFSXH    36
#define DMP_BLPFSXL    38
#define DMP_BLPFSYH    40
#define DMP_BLPFSYL    42
#define DMP_BLPFSZH    44
#define DMP_BLPFSZL    46
#define DMP_BLPFMTC    48
#define DMP_SMC    50
#define DMP_BLPFMXH    52
#define DMP_BLPFMXL    54
#define DMP_BLPFMYH    56
#define DMP_BLPFMYL    58
#define DMP_BLPFMZH    60
#define DMP_BLPFMZL    62
#define DMP_BLPFC    64
#define DMP_SMCTH    66
#define DMP_0H2    68
#define DMP_0L2    70
#define DMP_BERR2H    72
#define DMP_BERR2L    74
#define DMP_BERR2NH    76
#define DMP_SMCINC    78
#define DMP_ANGVBXH    80
#define DMP_ANGVBXL    82
#define DMP_ANGVBYH    84
#define DMP_ANGVBYL    86
#define DMP_ANGVBZH    88
#define DMP_ANGVBZL    90
#define DMP_BERR1H    92
#define DMP_BERR1L    94
#define DMP_ATCH    96
#define DMP_BIASUNCSF    98
#define DMP_ACT2H    100
#define DMP_ACT2L    102
#define DMP_GSFH    104
#define DMP_GSFL    106
#define DMP_GH    108
#define DMP_GL    110
#define DMP_0_5H    112
#define DMP_0_5L    114
#define DMP_0_0H    116
#define DMP_0_0L    118
#define DMP_1_0H    120
#define DMP_1_0L    122
#define DMP_1_5H    124
#define DMP_1_5L    126
#define DMP_TMP1AH    128
#define DMP_TMP1AL    130
#define DMP_TMP2AH    132
#define DMP_TMP2AL    134
#define DMP_TMP3AH    136
#define DMP_TMP3AL    138
#define DMP_TMP4AH    140
#define DMP_TMP4AL    142
#define DMP_XACCW    144
#define DMP_TMP5    146
#define DMP_XACCB    148
#define DMP_TMP8    150
#define DMP_YACCB    152
#define DMP_TMP9    154
#define DMP_ZACCB    156
#define DMP_TMP10    158
#define DMP_DZH    160
#define DMP_DZL    162
#define DMP_XGCH    164
#define DMP_XGCL    166
#define DMP_YGCH    168
#define DMP_YGCL    170
#define DMP_ZGCH    172
#define DMP_ZGCL    174
#define DMP_YACCW    176
#define DMP_TMP7    178
#define DMP_AFB1H    180
#define DMP_AFB1L    182
#define DMP_AFB2H    184
#define DMP_AFB2L    186
#define DMP_MAGFBH    188
#define DMP_MAGFBL    190
#define DMP_QT1H    192
#define DMP_QT1L    194
#define DMP_QT2H    196
#define DMP_QT2L    198
#define DMP_QT3H    200
#define DMP_QT3L    202
#define DMP_QT4H    204
#define DMP_QT4L    206
#define DMP_CTRL1H    208
#define DMP_CTRL1L    210
#define DMP_CTRL2H    212
#define DMP_CTRL2L    214
#define DMP_CTRL3H    216
#define DMP_CTRL3L    218
#define DMP_CTRL4H    220
#define DMP_CTRL4L    222
#define DMP_CTRLS1    224
#define DMP_CTRLSF1    226
#define DMP_CTRLS2    228
#define DMP_CTRLSF2    230
#define DMP_CTRLS3    232
#define DMP_CTRLSFNLL    234
#define DMP_CTRLS4    236
#define DMP_CTRLSFNL2    238
#define DMP_CTRLSFNL    240
#define DMP_TMP30    242
#define DMP_CTRLSFJT    244
#define DMP_TMP31    246
#define DMP_TMP11    248
#define DMP_CTRLSF2_2    250
#define DMP_TMP12    252
#define DMP_CTRLSF1_2    254
#define DMP_PREVPTAT    256
#define DMP_ACCZB    258
#define DMP_ACCXB    264
#define DMP_ACCYB    266
#define DMP_1HB    272
#define DMP_1LB    274
#define DMP_0H    276
#define DMP_0L    278
#define DMP_ASR22H    280
#define DMP_ASR22L    282
#define DMP_ASR6H    284
#define DMP_ASR6L    286
#define DMP_TMP13    288
#define DMP_TMP14    290
#define DMP_FINTXH    292
#define DMP_FINTXL    294
#define DMP_FINTYH    296
#define DMP_FINTYL    298
#define DMP_FINTZH    300
#define DMP_FINTZL    302
#define DMP_TMP1BH    304
#define DMP_TMP1BL    306
#define DMP_TMP2BH    308
#define DMP_TMP2BL    310
#define DMP_TMP3BH    312
#define DMP_TMP3BL    314
#define DMP_TMP4BH    316
#define DMP_TMP4BL    318
#define DMP_STXG    320
#define DMP_ZCTXG    322
#define DMP_STYG    324
#define DMP_ZCTYG    326
#define DMP_STZG    328
#define DMP_ZCTZG    330
#define DMP_CTRLSFJT2    332
#define DMP_CTRLSFJTCNT    334
#define DMP_PVXG    336
#define DMP_TMP15    338
#define DMP_PVYG    340
#define DMP_TMP16    342
#define DMP_PVZG    344
#define DMP_TMP17    346
#define DMP_MNMFLAGH    352
#define DMP_MNMFLAGL    354
#define DMP_MNMTMH    356
#define DMP_MNMTML    358
#define DMP_MNMTMTHRH    360
#define DMP_MNMTMTHRL    362
#define DMP_MNMTHRH    364
#define DMP_MNMTHRL    366
#define DMP_ACCQD4H    368
#define DMP_ACCQD4L    370
#define DMP_ACCQD5H    372
#define DMP_ACCQD5L    374
#define DMP_ACCQD6H    376
#define DMP_ACCQD6L    378
#define DMP_ACCQD7H    380
#define DMP_ACCQD7L    382
#define DMP_ACCQD0H    384
#define DMP_ACCQD0L    386
#define DMP_ACCQD1H    388
#define DMP_ACCQD1L    390
#define DMP_ACCQD2H    392
#define DMP_ACCQD2L    394
#define DMP_ACCQD3H    396
#define DMP_ACCQD3L    398
#define DMP_XN2H    400
#define DMP_XN2L    402
#define DMP_XN1H    404
#define DMP_XN1L    406
#define DMP_YN2H    408
#define DMP_YN2L    410
#define DMP_YN1H    412
#define DMP_YN1L    414
#define DMP_YH    416
#define DMP_YL    418
#define DMP_B0H    420
#define DMP_B0L    422
#define DMP_A1H    424
#define DMP_A1L    426
#define DMP_A2H    428
#define DMP_A2L    430
#define DMP_SEM1    432
#define DMP_FIFOCNT    434
#define DMP_SH_TH_X    436
#define DMP_PACKET    438
#define DMP_SH_TH_Y    440
#define DMP_FOOTER    442
#define DMP_SH_TH_Z    444
#define DMP_TEMP29    448
#define DMP_TEMP30    450
#define DMP_XACCB_PRE    452
#define DMP_XACCB_PREL    454
#define DMP_YACCB_PRE    456
#define DMP_YACCB_PREL    458
#define DMP_ZACCB_PRE    460
#define DMP_ZACCB_PREL    462
#define DMP_TMP22    464
#define DMP_TAP_TIMER    466
#define DMP_TAP_THX    468
#define DMP_TAP_THY    472
#define DMP_TAP_THZ    476
#define DMP_TAPW_MIN    478
#define DMP_TMP25    480
#define DMP_TMP26    482
#define DMP_TMP27    484
#define DMP_TMP28    486
#define DMP_ORIENT    488
#define DMP_THRSH    490
#define DMP_ENDIANH    492
#define DMP_ENDIANL    494
#define DMP_BLPFNMTCH    496
#define DMP_BLPFNMTCL    498
#define DMP_BLPFNMXH    500
#define DMP_BLPFNMXL    502
#define DMP_BLPFNMYH    504
#define DMP_BLPFNMYL    506
#define DMP_BLPFNMZH    508
#define DMP_BLPFNMZL    510

#define KEY_CFG_25                  (0)
#define KEY_CFG_24                  (KEY_CFG_25 + 1)
#define KEY_CFG_26                  (KEY_CFG_24 + 1)
#define KEY_CFG_27                  (KEY_CFG_26 + 1)
#define KEY_CFG_21                  (KEY_CFG_27 + 1)
#define KEY_CFG_20                  (KEY_CFG_21 + 1)
#define KEY_CFG_TAP4                (KEY_CFG_20 + 1)
#define KEY_CFG_TAP5                (KEY_CFG_TAP4 + 1)
#define KEY_CFG_TAP6                (KEY_CFG_TAP5 + 1)
#define KEY_CFG_TAP7                (KEY_CFG_TAP6 + 1)
#define KEY_CFG_TAP0                (KEY_CFG_TAP7 + 1)
#define KEY_CFG_TAP1                (KEY_CFG_TAP0 + 1)
#define KEY_CFG_TAP2                (KEY_CFG_TAP1 + 1)
#define KEY_CFG_TAP3                (KEY_CFG_TAP2 + 1)
#define KEY_CFG_TAP_QUANTIZE        (KEY_CFG_TAP3 + 1)
#define KEY_CFG_TAP_JERK            (KEY_CFG_TAP_QUANTIZE + 1)
#define KEY_CFG_DR_INT              (KEY_CFG_TAP_JERK + 1)
#define KEY_CFG_AUTH                (KEY_CFG_DR_INT + 1)
#define KEY_CFG_TAP_SAVE_ACCB       (KEY_CFG_AUTH + 1)
#define KEY_CFG_TAP_CLEAR_STICKY    (KEY_CFG_TAP_SAVE_ACCB + 1)
#define KEY_CFG_FIFO_ON_EVENT       (KEY_CFG_TAP_CLEAR_STICKY + 1)
#define KEY_FCFG_ACCEL_INPUT        (KEY_CFG_FIFO_ON_EVENT + 1)
#define KEY_FCFG_ACCEL_INIT         (KEY_FCFG_ACCEL_INPUT + 1)
#define KEY_CFG_23                  (KEY_FCFG_ACCEL_INIT + 1)
#define KEY_FCFG_1                  (KEY_CFG_23 + 1)
#define KEY_FCFG_3                  (KEY_FCFG_1 + 1)
#define KEY_FCFG_2                  (KEY_FCFG_3 + 1)
#define KEY_CFG_3D                  (KEY_FCFG_2 + 1)
#define KEY_CFG_3B                  (KEY_CFG_3D + 1)
#define KEY_CFG_3C                  (KEY_CFG_3B + 1)
#define KEY_FCFG_5                  (KEY_CFG_3C + 1)
#define KEY_FCFG_4                  (KEY_FCFG_5 + 1)
#define KEY_FCFG_7                  (KEY_FCFG_4 + 1)
#define KEY_FCFG_FSCALE             (KEY_FCFG_7 + 1)
#define KEY_FCFG_AZ                 (KEY_FCFG_FSCALE + 1)
#define KEY_FCFG_6                  (KEY_FCFG_AZ + 1)
#define KEY_FCFG_LSB4               (KEY_FCFG_6 + 1)
#define KEY_CFG_12                  (KEY_FCFG_LSB4 + 1)
#define KEY_CFG_14                  (KEY_CFG_12 + 1)
#define KEY_CFG_15                  (KEY_CFG_14 + 1)
#define KEY_CFG_16                  (KEY_CFG_15 + 1)
#define KEY_CFG_18                  (KEY_CFG_16 + 1)
#define KEY_CFG_6                   (KEY_CFG_18 + 1)
#define KEY_CFG_7                   (KEY_CFG_6 + 1)
#define KEY_CFG_4                   (KEY_CFG_7 + 1)
#define KEY_CFG_5                   (KEY_CFG_4 + 1)
#define KEY_CFG_2                   (KEY_CFG_5 + 1)
#define KEY_CFG_3                   (KEY_CFG_2 + 1)
#define KEY_CFG_1                   (KEY_CFG_3 + 1)
#define KEY_CFG_EXTERNAL            (KEY_CFG_1 + 1)
#define KEY_CFG_8                   (KEY_CFG_EXTERNAL + 1)
#define KEY_CFG_9                   (KEY_CFG_8 + 1)
#define KEY_CFG_ORIENT_3            (KEY_CFG_9 + 1)
#define KEY_CFG_ORIENT_2            (KEY_CFG_ORIENT_3 + 1)
#define KEY_CFG_ORIENT_1            (KEY_CFG_ORIENT_2 + 1)
#define KEY_CFG_GYRO_SOURCE         (KEY_CFG_ORIENT_1 + 1)
#define KEY_CFG_ORIENT_IRQ_1        (KEY_CFG_GYRO_SOURCE + 1)
#define KEY_CFG_ORIENT_IRQ_2        (KEY_CFG_ORIENT_IRQ_1 + 1)
#define KEY_CFG_ORIENT_IRQ_3        (KEY_CFG_ORIENT_IRQ_2 + 1)
#define KEY_FCFG_MAG_VAL            (KEY_CFG_ORIENT_IRQ_3 + 1)
#define KEY_FCFG_MAG_MOV            (KEY_FCFG_MAG_VAL + 1)
#define KEY_CFG_LP_QUAT             (KEY_FCFG_MAG_MOV + 1)

/* MPU6050 keys */
#define KEY_CFG_ACCEL_FILTER        (KEY_CFG_LP_QUAT + 1)
#define KEY_CFG_MOTION_BIAS         (KEY_CFG_ACCEL_FILTER + 1)
#define KEY_TEMPLABEL               (KEY_CFG_MOTION_BIAS + 1)

#define KEY_D_0_22                  (KEY_TEMPLABEL + 1)
#define KEY_D_0_24                  (KEY_D_0_22 + 1)
#define KEY_D_0_36                  (KEY_D_0_24 + 1)
#define KEY_D_0_52                  (KEY_D_0_36 + 1)
#define KEY_D_0_96                  (KEY_D_0_52 + 1)
#define KEY_D_0_104                 (KEY_D_0_96 + 1)
#define KEY_D_0_108                 (KEY_D_0_104 + 1)
#define KEY_D_0_163                 (KEY_D_0_108 + 1)
#define KEY_D_0_188                 (KEY_D_0_163 + 1)
#define KEY_D_0_192                 (KEY_D_0_188 + 1)
#define KEY_D_0_224                 (KEY_D_0_192 + 1)
#define KEY_D_0_228                 (KEY_D_0_224 + 1)
#define KEY_D_0_232                 (KEY_D_0_228 + 1)
#define KEY_D_0_236                 (KEY_D_0_232 + 1)

#define KEY_DMP_PREVPTAT            (KEY_D_0_236 + 1)
#define KEY_D_1_2                   (KEY_DMP_PREVPTAT + 1)
#define KEY_D_1_4                   (KEY_D_1_2 + 1)
#define KEY_D_1_8                   (KEY_D_1_4 + 1)
#define KEY_D_1_10                  (KEY_D_1_8 + 1)
#define KEY_D_1_24                  (KEY_D_1_10 + 1)
#define KEY_D_1_28                  (KEY_D_1_24 + 1)
#define KEY_D_1_36                  (KEY_D_1_28 + 1)
#define KEY_D_1_40                  (KEY_D_1_36 + 1)
#define KEY_D_1_44                  (KEY_D_1_40 + 1)
#define KEY_D_1_72                  (KEY_D_1_44 + 1)
#define KEY_D_1_74                  (KEY_D_1_72 + 1)
#define KEY_D_1_79                  (KEY_D_1_74 + 1)
#define KEY_D_1_88                  (KEY_D_1_79 + 1)
#define KEY_D_1_90                  (KEY_D_1_88 + 1)
#define KEY_D_1_92                  (KEY_D_1_90 + 1)
#define KEY_D_1_96                  (KEY_D_1_92 + 1)
#define KEY_D_1_98                  (KEY_D_1_96 + 1)
#define KEY_D_1_100                 (KEY_D_1_98 + 1)
#define KEY_D_1_106                 (KEY_D_1_100 + 1)
#define KEY_D_1_108                 (KEY_D_1_106 + 1)
#define KEY_D_1_112                 (KEY_D_1_108 + 1)
#define KEY_D_1_128                 (KEY_D_1_112 + 1)
#define KEY_D_1_152                 (KEY_D_1_128 + 1)
#define KEY_D_1_160                 (KEY_D_1_152 + 1)
#define KEY_D_1_168                 (KEY_D_1_160 + 1)
#define KEY_D_1_175                 (KEY_D_1_168 + 1)
#define KEY_D_1_176                 (KEY_D_1_175 + 1)
#define KEY_D_1_178                 (KEY_D_1_176 + 1)
#define KEY_D_1_179                 (KEY_D_1_178 + 1)
#define KEY_D_1_218                 (KEY_D_1_179 + 1)
#define KEY_D_1_232                 (KEY_D_1_218 + 1)
#define KEY_D_1_236                 (KEY_D_1_232 + 1)
#define KEY_D_1_240                 (KEY_D_1_236 + 1)
#define KEY_D_1_244                 (KEY_D_1_240 + 1)
#define KEY_D_1_250                 (KEY_D_1_244 + 1)
#define KEY_D_1_252                 (KEY_D_1_250 + 1)
#define KEY_D_2_12                  (KEY_D_1_252 + 1)
#define KEY_D_2_96                  (KEY_D_2_12 + 1)
#define KEY_D_2_108                 (KEY_D_2_96 + 1)
#define KEY_D_2_208                 (KEY_D_2_108 + 1)
#define KEY_FLICK_MSG               (KEY_D_2_208 + 1)
#define KEY_FLICK_COUNTER           (KEY_FLICK_MSG + 1)
#define KEY_FLICK_LOWER             (KEY_FLICK_COUNTER + 1)
#define KEY_CFG_FLICK_IN            (KEY_FLICK_LOWER + 1)
#define KEY_FLICK_UPPER             (KEY_CFG_FLICK_IN + 1)
#define KEY_CGNOTICE_INTR           (KEY_FLICK_UPPER + 1)
#define KEY_D_2_224                 (KEY_CGNOTICE_INTR + 1)
#define KEY_D_2_244                 (KEY_D_2_224 + 1)
#define KEY_D_2_248                 (KEY_D_2_244 + 1)
#define KEY_D_2_252                 (KEY_D_2_248 + 1)

#define KEY_D_GYRO_BIAS_X               (KEY_D_2_252 + 1)
#define KEY_D_GYRO_BIAS_Y               (KEY_D_GYRO_BIAS_X + 1)
#define KEY_D_GYRO_BIAS_Z               (KEY_D_GYRO_BIAS_Y + 1)
#define KEY_D_ACC_BIAS_X                (KEY_D_GYRO_BIAS_Z + 1)
#define KEY_D_ACC_BIAS_Y                (KEY_D_ACC_BIAS_X + 1)
#define KEY_D_ACC_BIAS_Z                (KEY_D_ACC_BIAS_Y + 1)
#define KEY_D_GYRO_ENABLE               (KEY_D_ACC_BIAS_Z + 1)
#define KEY_D_ACCEL_ENABLE              (KEY_D_GYRO_ENABLE + 1)
#define KEY_D_QUAT_ENABLE               (KEY_D_ACCEL_ENABLE +1)
#define KEY_D_OUTPUT_ENABLE             (KEY_D_QUAT_ENABLE + 1)
#define KEY_D_CR_TIME_G                 (KEY_D_OUTPUT_ENABLE + 1)
#define KEY_D_CR_TIME_A                 (KEY_D_CR_TIME_G + 1)
#define KEY_D_CR_TIME_Q                 (KEY_D_CR_TIME_A + 1)
#define KEY_D_CS_TAX                    (KEY_D_CR_TIME_Q + 1)
#define KEY_D_CS_TAY                    (KEY_D_CS_TAX + 1)
#define KEY_D_CS_TAZ                    (KEY_D_CS_TAY + 1)
#define KEY_D_CS_TGX                    (KEY_D_CS_TAZ + 1)
#define KEY_D_CS_TGY                    (KEY_D_CS_TGX + 1)
#define KEY_D_CS_TGZ                    (KEY_D_CS_TGY + 1)
#define KEY_D_CS_TQ0                    (KEY_D_CS_TGZ + 1)
#define KEY_D_CS_TQ1                    (KEY_D_CS_TQ0 + 1)
#define KEY_D_CS_TQ2                    (KEY_D_CS_TQ1 + 1)
#define KEY_D_CS_TQ3                    (KEY_D_CS_TQ2 + 1)

/* Compass keys */
#define KEY_CPASS_BIAS_X            (KEY_D_CS_TQ3 + 1)
#define KEY_CPASS_BIAS_Y            (KEY_CPASS_BIAS_X + 1)
#define KEY_CPASS_BIAS_Z            (KEY_CPASS_BIAS_Y + 1)
#define KEY_CPASS_MTX_00            (KEY_CPASS_BIAS_Z + 1)
#define KEY_CPASS_MTX_01            (KEY_CPASS_MTX_00 + 1)
#define KEY_CPASS_MTX_02            (KEY_CPASS_MTX_01 + 1)
#define KEY_CPASS_MTX_10            (KEY_CPASS_MTX_02 + 1)
#define KEY_CPASS_MTX_11            (KEY_CPASS_MTX_10 + 1)
#define KEY_CPASS_MTX_12            (KEY_CPASS_MTX_11 + 1)
#define KEY_CPASS_MTX_20            (KEY_CPASS_MTX_12 + 1)
#define KEY_CPASS_MTX_21            (KEY_CPASS_MTX_20 + 1)
#define KEY_CPASS_MTX_22            (KEY_CPASS_MTX_21 + 1)

/* Gesture Keys */
#define KEY_DMP_TAPW_MIN            (KEY_CPASS_MTX_22 + 1)
#define KEY_DMP_TAP_THR_X           (KEY_DMP_TAPW_MIN + 1)
#define KEY_DMP_TAP_THR_Y           (KEY_DMP_TAP_THR_X + 1)
#define KEY_DMP_TAP_THR_Z           (KEY_DMP_TAP_THR_Y + 1)
#define KEY_DMP_SH_TH_Y             (KEY_DMP_TAP_THR_Z + 1)
#define KEY_DMP_SH_TH_X             (KEY_DMP_SH_TH_Y + 1)
#define KEY_DMP_SH_TH_Z             (KEY_DMP_SH_TH_X + 1)
#define KEY_DMP_ORIENT              (KEY_DMP_SH_TH_Z + 1)
#define KEY_D_ACT0                  (KEY_DMP_ORIENT + 1)
#define KEY_D_ACSX                  (KEY_D_ACT0 + 1)
#define KEY_D_ACSY                  (KEY_D_ACSX + 1)
#define KEY_D_ACSZ                  (KEY_D_ACSY + 1)

#define KEY_X_GRT_Y_TMP             (KEY_D_ACSZ + 1)
#define KEY_SKIP_X_GRT_Y_TMP        (KEY_X_GRT_Y_TMP + 1)
#define KEY_SKIP_END_COMPARE        (KEY_SKIP_X_GRT_Y_TMP + 1)
#define KEY_END_COMPARE_Y_X_TMP2    (KEY_SKIP_END_COMPARE + 1)
#define KEY_CFG_ANDROID_ORIENT_INT  (KEY_END_COMPARE_Y_X_TMP2 + 1)
#define KEY_NO_ORIENT_INTERRUPT     (KEY_CFG_ANDROID_ORIENT_INT + 1)
#define KEY_END_COMPARE_Y_X_TMP     (KEY_NO_ORIENT_INTERRUPT + 1)
#define KEY_END_ORIENT_1            (KEY_END_COMPARE_Y_X_TMP + 1)
#define KEY_END_COMPARE_Y_X         (KEY_END_ORIENT_1 + 1)
#define KEY_END_ORIENT              (KEY_END_COMPARE_Y_X + 1)
#define KEY_X_GRT_Y                 (KEY_END_ORIENT + 1)
#define KEY_NOT_TIME_MINUS_1        (KEY_X_GRT_Y + 1)
#define KEY_END_COMPARE_Y_X_TMP3    (KEY_NOT_TIME_MINUS_1 + 1)
#define KEY_X_GRT_Y_TMP2            (KEY_END_COMPARE_Y_X_TMP3 + 1)

/* Authenticate Keys */
#define KEY_D_AUTH_OUT              (KEY_X_GRT_Y_TMP2 + 1)
#define KEY_D_AUTH_IN               (KEY_D_AUTH_OUT + 1)
#define KEY_D_AUTH_A                (KEY_D_AUTH_IN + 1)
#define KEY_D_AUTH_B                (KEY_D_AUTH_A + 1)

/* Pedometer standalone only keys */
#define KEY_D_PEDSTD_BP_B           (KEY_D_AUTH_B + 1)
#define KEY_D_PEDSTD_HP_A           (KEY_D_PEDSTD_BP_B + 1)
#define KEY_D_PEDSTD_HP_B           (KEY_D_PEDSTD_HP_A + 1)
#define KEY_D_PEDSTD_BP_A4          (KEY_D_PEDSTD_HP_B + 1)
#define KEY_D_PEDSTD_BP_A3          (KEY_D_PEDSTD_BP_A4 + 1)
#define KEY_D_PEDSTD_BP_A2          (KEY_D_PEDSTD_BP_A3 + 1)
#define KEY_D_PEDSTD_BP_A1          (KEY_D_PEDSTD_BP_A2 + 1)
#define KEY_D_PEDSTD_INT_THRSH      (KEY_D_PEDSTD_BP_A1 + 1)
#define KEY_D_PEDSTD_CLIP           (KEY_D_PEDSTD_INT_THRSH + 1)
#define KEY_D_PEDSTD_SB             (KEY_D_PEDSTD_CLIP + 1)
#define KEY_D_PEDSTD_SB_TIME        (KEY_D_PEDSTD_SB + 1)
#define KEY_D_PEDSTD_PEAKTHRSH      (KEY_D_PEDSTD_SB_TIME + 1)
#define KEY_D_PEDSTD_TIML           (KEY_D_PEDSTD_PEAKTHRSH + 1)
#define KEY_D_PEDSTD_TIMH           (KEY_D_PEDSTD_TIML + 1)
#define KEY_D_PEDSTD_PEAK           (KEY_D_PEDSTD_TIMH + 1)
#define KEY_D_PEDSTD_TIMECTR        (KEY_D_PEDSTD_PEAK + 1)
#define KEY_D_PEDSTD_STEPCTR        (KEY_D_PEDSTD_TIMECTR + 1)
#define KEY_D_PEDSTD_WALKTIME       (KEY_D_PEDSTD_STEPCTR + 1)
#define KEY_D_PEDSTD_DECI           (KEY_D_PEDSTD_WALKTIME + 1)

/*Host Based No Motion*/
#define KEY_D_HOST_NO_MOT           (KEY_D_PEDSTD_DECI + 1)

/* EIS keys */
#define KEY_P_EIS_FIFO_FOOTER       (KEY_D_HOST_NO_MOT + 1)
#define KEY_P_EIS_FIFO_YSHIFT       (KEY_P_EIS_FIFO_FOOTER + 1)
#define KEY_P_EIS_DATA_RATE         (KEY_P_EIS_FIFO_YSHIFT + 1)
#define KEY_P_EIS_FIFO_XSHIFT       (KEY_P_EIS_DATA_RATE + 1)
#define KEY_P_EIS_FIFO_SYNC         (KEY_P_EIS_FIFO_XSHIFT + 1)
#define KEY_P_EIS_FIFO_ZSHIFT       (KEY_P_EIS_FIFO_SYNC + 1)
#define KEY_P_EIS_FIFO_READY        (KEY_P_EIS_FIFO_ZSHIFT + 1)
#define KEY_DMP_FOOTER              (KEY_P_EIS_FIFO_READY + 1)
#define KEY_DMP_INTX_HC             (KEY_DMP_FOOTER + 1)
#define KEY_DMP_INTX_PH             (KEY_DMP_INTX_HC + 1)
#define KEY_DMP_INTX_SH             (KEY_DMP_INTX_PH + 1)
#define KEY_DMP_AINV_SH             (KEY_DMP_INTX_SH + 1)
#define KEY_DMP_A_INV_XH            (KEY_DMP_AINV_SH + 1)
#define KEY_DMP_AINV_PH             (KEY_DMP_A_INV_XH + 1)
#define KEY_DMP_CTHX_H              (KEY_DMP_AINV_PH + 1)
#define KEY_DMP_CTHY_H              (KEY_DMP_CTHX_H + 1)
#define KEY_DMP_CTHZ_H              (KEY_DMP_CTHY_H + 1)
#define KEY_DMP_NCTHX_H             (KEY_DMP_CTHZ_H + 1)
#define KEY_DMP_NCTHY_H             (KEY_DMP_NCTHX_H + 1)
#define KEY_DMP_NCTHZ_H             (KEY_DMP_NCTHY_H + 1)
#define KEY_DMP_CTSQ_XH             (KEY_DMP_NCTHZ_H + 1)
#define KEY_DMP_CTSQ_YH             (KEY_DMP_CTSQ_XH + 1)
#define KEY_DMP_CTSQ_ZH             (KEY_DMP_CTSQ_YH + 1)
#define KEY_DMP_INTX_H              (KEY_DMP_CTSQ_ZH + 1)
#define KEY_DMP_INTY_H              (KEY_DMP_INTX_H + 1)
#define KEY_DMP_INTZ_H              (KEY_DMP_INTY_H + 1)
//#define KEY_DMP_HPX_H               (KEY_DMP_INTZ_H + 1)
//#define KEY_DMP_HPY_H               (KEY_DMP_HPX_H + 1)
//#define KEY_DMP_HPZ_H               (KEY_DMP_HPY_H + 1)

/* Stream keys */
#define KEY_STREAM_P_GYRO_Z         (KEY_DMP_INTZ_H + 1)
#define KEY_STREAM_P_GYRO_Y         (KEY_STREAM_P_GYRO_Z + 1)
#define KEY_STREAM_P_GYRO_X         (KEY_STREAM_P_GYRO_Y + 1)
#define KEY_STREAM_P_TEMP           (KEY_STREAM_P_GYRO_X + 1)
#define KEY_STREAM_P_AUX_Y          (KEY_STREAM_P_TEMP + 1)
#define KEY_STREAM_P_AUX_X          (KEY_STREAM_P_AUX_Y + 1)
#define KEY_STREAM_P_AUX_Z          (KEY_STREAM_P_AUX_X + 1)
#define KEY_STREAM_P_ACCEL_Y        (KEY_STREAM_P_AUX_Z + 1)
#define KEY_STREAM_P_ACCEL_X        (KEY_STREAM_P_ACCEL_Y + 1)
#define KEY_STREAM_P_FOOTER         (KEY_STREAM_P_ACCEL_X + 1)
#define KEY_STREAM_P_ACCEL_Z        (KEY_STREAM_P_FOOTER + 1)

#define NUM_KEYS                    (KEY_STREAM_P_ACCEL_Z + 1)


#define DINA0A 0x0a
#define DINA22 0x22
#define DINA42 0x42
#define DINA5A 0x5a

#define DINA06 0x06
#define DINA0E 0x0e
#define DINA16 0x16
#define DINA1E 0x1e
#define DINA26 0x26
#define DINA2E 0x2e
#define DINA36 0x36
#define DINA3E 0x3e
#define DINA46 0x46
#define DINA4E 0x4e
#define DINA56 0x56
#define DINA5E 0x5e
#define DINA66 0x66
#define DINA6E 0x6e
#define DINA76 0x76
#define DINA7E 0x7e

#define DINA00 0x00
#define DINA08 0x08
#define DINA10 0x10
#define DINA18 0x18
#define DINA20 0x20
#define DINA28 0x28
#define DINA30 0x30
#define DINA38 0x38
#define DINA40 0x40
#define DINA48 0x48
#define DINA50 0x50
#define DINA58 0x58
#define DINA60 0x60
#define DINA68 0x68
#define DINA70 0x70
#define DINA78 0x78

#define DINA04 0x04
#define DINA0C 0x0c
#define DINA14 0x14
#define DINA1C 0x1C
#define DINA24 0x24
#define DINA2C 0x2c
#define DINA34 0x34
#define DINA3C 0x3c
#define DINA44 0x44
#define DINA4C 0x4c
#define DINA54 0x54
#define DINA5C 0x5c
#define DINA64 0x64
#define DINA6C 0x6c
#define DINA74 0x74
#define DINA7C 0x7c

#define DINA01 0x01
#define DINA09 0x09
#define DINA11 0x11
#define DINA19 0x19
#define DINA21 0x21
#define DINA29 0x29
#define DINA31 0x31
#define DINA39 0x39
#define DINA41 0x41
#define DINA49 0x49
#define DINA51 0x51
#define DINA59 0x59
#define DINA61 0x61
#define DINA69 0x69
#define DINA71 0x71
#define DINA79 0x79

#define DINA25 0x25
#define DINA2D 0x2d
#define DINA35 0x35
#define DINA3D 0x3d
#define DINA4D 0x4d
#define DINA55 0x55
#define DINA5D 0x5D
#define DINA6D 0x6d
#define DINA75 0x75
#define DINA7D 0x7d

#define DINADC 0xdc
#define DINAF2 0xf2
#define DINAAB 0xab
#define DINAAA 0xaa
#define DINAF1 0xf1
#define DINADF 0xdf
#define DINADA 0xda
#define DINAB1 0xb1
#define DINAB9 0xb9
#define DINAF3 0xf3
#define DINA8B 0x8b
#define DINAA3 0xa3
#define DINA91 0x91
#define DINAB6 0xb6
#define DINAB4 0xb4


#define DINC00 0x00
#define DINC01 0x01
#define DINC02 0x02
#define DINC03 0x03
#define DINC08 0x08
#define DINC09 0x09
#define DINC0A 0x0a
#define DINC0B 0x0b
#define DINC10 0x10
#define DINC11 0x11
#define DINC12 0x12
#define DINC13 0x13
#define DINC18 0x18
#define DINC19 0x19
#define DINC1A 0x1a
#define DINC1B 0x1b

#define DINC20 0x20
#define DINC21 0x21
#define DINC22 0x22
#define DINC23 0x23
#define DINC28 0x28
#define DINC29 0x29
#define DINC2A 0x2a
#define DINC2B 0x2b
#define DINC30 0x30
#define DINC31 0x31
#define DINC32 0x32
#define DINC33 0x33
#define DINC38 0x38
#define DINC39 0x39
#define DINC3A 0x3a
#define DINC3B 0x3b

#define DINC40 0x40
#define DINC41 0x41
#define DINC42 0x42
#define DINC43 0x43
#define DINC48 0x48
#define DINC49 0x49
#define DINC4A 0x4a
#define DINC4B 0x4b
#define DINC50 0x50
#define DINC51 0x51
#define DINC52 0x52
#define DINC53 0x53
#define DINC58 0x58
#define DINC59 0x59
#define DINC5A 0x5a
#define DINC5B 0x5b

#define DINC60 0x60
#define DINC61 0x61
#define DINC62 0x62
#define DINC63 0x63
#define DINC68 0x68
#define DINC69 0x69
#define DINC6A 0x6a
#define DINC6B 0x6b
#define DINC70 0x70
#define DINC71 0x71
#define DINC72 0x72
#define DINC73 0x73
#define DINC78 0x78
#define DINC79 0x79
#define DINC7A 0x7a
#define DINC7B 0x7b

#define DIND40 0x40


#define DINA80 0x80
#define DINA90 0x90
#define DINAA0 0xa0
#define DINAC9 0xc9
#define DINACB 0xcb
#define DINACD 0xcd
#define DINACF 0xcf
#define DINAC8 0xc8
#define DINACA 0xca
#define DINACC 0xcc
#define DINACE 0xce
#define DINAD8 0xd8
#define DINADD 0xdd
#define DINAF8 0xf0
#define DINAFE 0xfe

#define DINBF8 0xf8
#define DINAC0 0xb0
#define DINAC1 0xb1
#define DINAC2 0xb4
#define DINAC3 0xb5
#define DINAC4 0xb8
#define DINAC5 0xb9
#define DINBC0 0xc0
#define DINBC2 0xc2
#define DINBC4 0xc4
#define DINBC6 0xc6

#define INT_SRC_TAP             (0x01)
#define INT_SRC_ANDROID_ORIENT  (0x08)

#define DMP_FEATURE_SEND_ANY_GYRO   (DMP_FEATURE_SEND_RAW_GYRO | \
                                     DMP_FEATURE_SEND_CAL_GYRO)

#define MAX_PACKET_LENGTH   (32)

#define DMP_SAMPLE_RATE     (200)
#define GYRO_SF             (46850825LL * 200 / DMP_SAMPLE_RATE)

#define FIFO_CORRUPTION_CHECK
#ifdef FIFO_CORRUPTION_CHECK
#define QUAT_ERROR_THRESH       (1L<<24)
#define QUAT_MAG_SQ_NORMALIZED  (1L<<28)
#define QUAT_MAG_SQ_MIN         (QUAT_MAG_SQ_NORMALIZED - QUAT_ERROR_THRESH)
#define QUAT_MAG_SQ_MAX         (QUAT_MAG_SQ_NORMALIZED + QUAT_ERROR_THRESH)
#endif

#define GYRO_SENS       16.375f 
#define QUAT_SENS       1073741824.f //2^30

#define MAX_COMPASS_SAMPLE_RATE (100)


//******************************************************************************
//* Constructors
//******************************************************************************
MPUSeries::MPUSeries(int id, int model, boolean secondary, signed char *orientMatrix) : ArdunityI2C(id)
{
    _model = model;
    _orientMatrix = orientMatrix;
    if(secondary)
        address = 0x69;
    else
        address = 0x68;
    
    _qX = 0;
    _qY = 0;
    _qZ = 0;
    _qW = 1;
    _initialized = false;
	canFlush = true;
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void MPUSeries::OnSetup()
{
	ArdunityI2C::OnSetup();
	
	_initialized = initialize();
}

void MPUSeries::OnStart()
{
}

void MPUSeries::OnStop()
{
}

void MPUSeries::OnProcess()
{
	if(started && _initialized)
    {
        if(update()) // Successful MPU DMP read
        {
            _qX = mympu.xyzw[0];
            _qY = mympu.xyzw[1];
            _qZ = mympu.xyzw[2];
            _qW = mympu.xyzw[3];

            dirty = true;
        }
    }
}

void MPUSeries::OnUpdate()
{
}

void MPUSeries::OnExecute()
{
}

void MPUSeries::OnFlush()
{
	ArdunityApp.push(_qX);
    ArdunityApp.push(_qY);
    ArdunityApp.push(_qZ);
	ArdunityApp.push(_qW);
}

//******************************************************************************
//* Private Methods
//******************************************************************************
boolean MPUSeries::initialize()
{
    if(_model == MPU6050 || _model == MPU9150)
    {
        reg.who_am_i         = 0x75;
        reg.rate_div         = 0x19;
        reg.lpf              = 0x1A;
        reg.prod_id          = 0x0C;
        reg.user_ctrl        = 0x6A;
        reg.fifo_en          = 0x23;
        reg.gyro_cfg         = 0x1B;
        reg.accel_cfg        = 0x1C;
        reg.motion_thr       = 0x1F;
        reg.motion_dur       = 0x20;
        reg.fifo_count_h     = 0x72;
        reg.fifo_r_w         = 0x74;
        reg.raw_gyro         = 0x43;
        reg.raw_accel        = 0x3B;
        reg.temp             = 0x41;
        reg.int_enable       = 0x38;
        reg.dmp_int_status   = 0x39;
        reg.int_status       = 0x3A;
        reg.pwr_mgmt_1       = 0x6B;
        reg.pwr_mgmt_2       = 0x6C;
        reg.int_pin_cfg      = 0x37;
        reg.mem_r_w          = 0x6F;
        reg.accel_offs       = 0x06;
        reg.i2c_mst          = 0x24;
        reg.bank_sel         = 0x6D;
        reg.mem_start_addr   = 0x6E;
        reg.prgm_start_h     = 0x70;
        if(_model == MPU9150)
        {
            reg.raw_compass      = 0x49;
            reg.yg_offs_tc       = 0x01;
            reg.s0_addr          = 0x25;
            reg.s0_reg           = 0x26;
            reg.s0_ctrl          = 0x27;
            reg.s1_addr          = 0x28;
            reg.s1_reg           = 0x29;
            reg.s1_ctrl          = 0x2A;
            reg.s4_ctrl          = 0x34;
            reg.s0_do            = 0x63;
            reg.s1_do            = 0x64;
            reg.i2c_delay_ctrl   = 0x67;
        }

        hw.addr 			 = address;
        hw.max_fifo          = 1024;
        hw.num_reg           = 118;
        hw.temp_sens         = 340;
        hw.temp_offset       = -521;
        hw.bank_size         = 256;
        if(_model == MPU9150)
            hw.compass_fsr = AK89xx_FSR;
    }
    else if(_model == MPU6500 || _model == MPU9250)
    {
        reg.who_am_i         = 0x75;
        reg.rate_div         = 0x19;
        reg.lpf              = 0x1A;
        reg.prod_id          = 0x0C;
        reg.user_ctrl        = 0x6A;
        reg.fifo_en          = 0x23;
        reg.gyro_cfg         = 0x1B;
        reg.accel_cfg        = 0x1C;
        reg.accel_cfg2        = 0x1D;
        reg.lp_accel_odr     = 0x1E;
        reg.motion_thr       = 0x1F;
        reg.motion_dur       = 0x20;
        reg.fifo_count_h     = 0x72;
        reg.fifo_r_w         = 0x74;
        reg.raw_gyro         = 0x43;
        reg.raw_accel        = 0x3B;
        reg.temp             = 0x41;
        reg.int_enable       = 0x38;
        reg.dmp_int_status   = 0x39;
        reg.int_status       = 0x3A;
        reg.accel_intel      = 0x69;
        reg.pwr_mgmt_1       = 0x6B;
        reg.pwr_mgmt_2       = 0x6C;
        reg.int_pin_cfg      = 0x37;
        reg.mem_r_w          = 0x6F;
        reg.accel_offs       = 0x77;
        reg.i2c_mst          = 0x24;
        reg.bank_sel         = 0x6D;
        reg.mem_start_addr   = 0x6E;
        reg.prgm_start_h     = 0x70;
        if(_model == MPU9250)
        {
            reg.raw_compass      = 0x49;
            reg.yg_offs_tc       = 0x01;
            reg.s0_addr          = 0x25;
            reg.s0_reg           = 0x26;
            reg.s0_ctrl          = 0x27;
            reg.s1_addr          = 0x28;
            reg.s1_reg           = 0x29;
            reg.s1_ctrl          = 0x2A;
            reg.s4_ctrl          = 0x34;
            reg.s0_do            = 0x63;
            reg.s1_do            = 0x64;
            reg.i2c_delay_ctrl   = 0x67;
        }
        
        hw.addr 			 = address;
        hw.max_fifo          = 1024;
        hw.num_reg           = 128;
        hw.temp_sens         = 321;
        hw.temp_offset       = 0;
        hw.bank_size         = 256;
        if(_model == MPU9250)
            hw.compass_fsr = AK89xx_FSR2;
    }
    else
        return false;
    
    test.gyro_sens      = 32768/250;
    test.accel_sens     = 32768/16;
    test.reg_rate_div   = 0;    /* 1kHz. */
    test.reg_lpf        = 1;    /* 188Hz. */
    test.reg_gyro_fsr   = 0;    /* 250dps. */
    test.reg_accel_fsr  = 0x18; /* 16g. */
    test.wait_ms        = 50;
    test.packet_thresh  = 5;    /* 5% */
    test.min_dps        = 10.f;
    test.max_dps        = 105.f;
    test.max_gyro_var   = 0.14f;
    test.min_g          = 0.3f;
    test.max_g          = 0.95f;
    test.max_accel_var  = 0.14f;

    st.reg = &reg;
    st.hw = &hw;
    st.test = &test;
	
	
	uint8_t data[6], rev;

    // Reset device.
    if(!Write(st.reg->pwr_mgmt_1, BIT_RESET))
        return false;
    delay(100);

    // Wake up chip.
    if(!Write(st.reg->pwr_mgmt_1, 0x00))
        return false;

    // Check product revision.
    if(_model == MPU6050 || _model == MPU9150)
    {
        if(!Read(st.reg->accel_offs, data, 6))
            return false;
        rev = ((data[5] & 0x01) << 2) | ((data[3] & 0x01) << 1) | (data[1] & 0x01);
        if(rev > 0)
        {
            // Congrats, these parts are better.
            if (rev == 1)
                st.chip_cfg.accel_half = 1;
            else if (rev == 2)
                st.chip_cfg.accel_half = 0;
            else
                return false; // Unsupported software product rev
        }
        else
        {
            if(!Read(st.reg->prod_id, data, 1))
                return false;
            
            rev = data[0] & 0x0F;
            if (rev == 0) // Product ID read as 0 indicates device is either incompatible or an MPU3050
                return false;
            else if (rev == 4)
                st.chip_cfg.accel_half = 1;
            else
                st.chip_cfg.accel_half = 0;
        }
    }
    else if(_model == MPU6500 || _model == MPU9250)
    {
  //      if (read_mem(0x17, 1, &rev)) // MPU6500_MEM_REV_ADDR    0x17
  //          return false;
        
  //      if (rev == 0x1)
            st.chip_cfg.accel_half = 0;
  //      else
  //          return false;

        // MPU6500 shares 4kB of memory between the DMP and the FIFO. Since the
        // first 3kB are needed by the DMP, we'll use the last 1kB for the FIFO.
        data[0] = BIT_FIFO_SIZE_1024 | 0x8;
        if (!Write(st.reg->accel_cfg2, data[0]))
            return false;
    }

    // Set to invalid values to ensure no I2C writes are skipped.
    st.chip_cfg.sensors = 0xFF;
    st.chip_cfg.gyro_fsr = 0xFF;
    st.chip_cfg.accel_fsr = 0xFF;
    st.chip_cfg.lpf = 0xFF;
    st.chip_cfg.sample_rate = 0xFFFF;
    st.chip_cfg.fifo_enable = 0xFF;
    st.chip_cfg.bypass_mode = 0xFF;

    // set_sensors always preserves this setting.
    st.chip_cfg.clk_src = INV_CLK_PLL;
    // Handled in next call to set_bypass.
    st.chip_cfg.active_low_int = 1;
    st.chip_cfg.latched_int = 0;
    st.chip_cfg.int_motion_only = 0;
    st.chip_cfg.lp_accel_mode = 0;
    memset(&st.chip_cfg.cache, 0, sizeof(st.chip_cfg.cache));
    st.chip_cfg.dmp_on = 0;
    st.chip_cfg.dmp_loaded = 0;
    st.chip_cfg.dmp_sample_rate = 0;

    if (set_gyro_fsr(2000))
        return false;
    if (set_accel_fsr(2))
        return false;
    if (set_lpf(42))
        return false;
    if (set_sample_rate(50))
        return false;
    if (configure_fifo(INV_XYZ_GYRO|INV_XYZ_ACCEL))
        return false;
    
    set_sensors(INV_XYZ_GYRO|INV_XYZ_ACCEL);
    
    dmp.tap_cb = NULL;
    dmp.android_orient_cb = NULL;
    dmp.orient = 0;
    dmp.feature_mask = 0;
    dmp.fifo_rate = 0;
    dmp.packet_length = 0;
  
	load_firmware(DMP_CODE_SIZE, dmp_memory, sStartAddress, DMP_SAMPLE_RATE);
    
	set_fifo_rate(60);
	set_dmp_state(1);
    
	set_orientation(orientation_matrix_to_scalar(_orientMatrix));
	enable_feature(DMP_FEATURE_6X_LP_QUAT|DMP_FEATURE_SEND_CAL_GYRO|DMP_FEATURE_GYRO_CAL);

    return true;
}

boolean MPUSeries::update()
{
    do {
		ret = read_fifo(gyro, NULL, q._l, NULL, &sensors, &fifoCount);
		// will return:
		//	0 - if ok
		//	1 - no packet available
		//	2 - if BIT_FIFO_OVERFLOW is set
		//	3 - if frame corrupted
		//       <0 - if error

		if (ret != 0)
            return false;
        
	} while (fifoCount > 1);

	q._f.w = (float)q._l[0] / (float)QUAT_SENS;
	q._f.x = (float)q._l[1] / (float)QUAT_SENS;
	q._f.y = (float)q._l[2] / (float)QUAT_SENS;
	q._f.z = (float)q._l[3] / (float)QUAT_SENS;

	mympu.xyzw[0] = q._f.x;
	mympu.xyzw[1] = q._f.y;
	mympu.xyzw[2] = q._f.z;
	mympu.xyzw[3] = q._f.w;

	// need to adjust signs depending on the MPU mount orientation
	mympu.gyro[0] = -(float)gyro[2] / GYRO_SENS;
	mympu.gyro[1] = (float)gyro[1] / GYRO_SENS;
	mympu.gyro[2] = (float)gyro[0] / GYRO_SENS;

	return true;
}

//  @brief      Set the gyro full-scale range.
//  @param[in]  fsr Desired full-scale range.
//  @return     0 if successful.
int MPUSeries::set_gyro_fsr(unsigned short fsr)
{
    unsigned char newFsr;

    if (!(st.chip_cfg.sensors))
        return -1;

    switch (fsr)
	{
    case 250:
        newFsr = INV_FSR_250DPS << 3;
        break;
    case 500:
        newFsr = INV_FSR_500DPS << 3;
        break;
    case 1000:
        newFsr = INV_FSR_1000DPS << 3;
        break;
    case 2000:
        newFsr = INV_FSR_2000DPS << 3;
        break;
    default:
        return -1;
    }

    if (st.chip_cfg.gyro_fsr == (newFsr >> 3))
        return 0;
	
    if(!Write(st.reg->gyro_cfg, newFsr))
        return -1;

    st.chip_cfg.gyro_fsr = newFsr >> 3;
    return 0;
}

//  @brief      Set the accel full-scale range.
//  @param[in]  fsr Desired full-scale range.
//  @return     0 if successful.
int MPUSeries::set_accel_fsr(unsigned char fsr)
{
    unsigned char newFsr;

    if (!(st.chip_cfg.sensors))
        return -1;

    switch (fsr)
	{
    case 2:
        newFsr = INV_FSR_2G << 3;
        break;
    case 4:
        newFsr = INV_FSR_4G << 3;
        break;
    case 8:
        newFsr = INV_FSR_8G << 3;
        break;
    case 16:
        newFsr = INV_FSR_16G << 3;
        break;
    default:
        return -1;
    }

    if (st.chip_cfg.accel_fsr == (newFsr >> 3))
        return 0;
	
    if(!Write(st.reg->accel_cfg, newFsr))
        return -1;
		
    st.chip_cfg.accel_fsr = newFsr >> 3;
    return 0;
}

//  @brief      Set digital low pass filter.
//  The following LPF settings are supported: 188, 98, 42, 20, 10, 5.
//  @param[in]  lpf Desired LPF setting.
//  @return     0 if successful.
int MPUSeries::set_lpf(unsigned short lpf)
{
    unsigned char newLpf;
    
    if (!(st.chip_cfg.sensors))
        return -1;

    if (lpf >= 188)
        newLpf = INV_FILTER_188HZ;
    else if (lpf >= 98)
        newLpf = INV_FILTER_98HZ;
    else if (lpf >= 42)
        newLpf = INV_FILTER_42HZ;
    else if (lpf >= 20)
        newLpf = INV_FILTER_20HZ;
    else if (lpf >= 10)
        newLpf = INV_FILTER_10HZ;
    else
        newLpf = INV_FILTER_5HZ;

    if (st.chip_cfg.lpf == newLpf)
        return 0;

    if(!Write(st.reg->lpf, newLpf))
        return -1;
    
    st.chip_cfg.lpf = newLpf;
    return 0;
}

//  @brief      Set sampling rate.
//  Sampling rate must be between 4Hz and 1kHz.
//  @param[in]  rate    Desired sampling rate (Hz).
//  @return     0 if successful.
int MPUSeries::set_sample_rate(unsigned short rate)
{
    unsigned char newRate;

    if (!(st.chip_cfg.sensors))
        return -1;

    if (st.chip_cfg.dmp_on)
        return -1;
    else
    {
        if (st.chip_cfg.lp_accel_mode)
        {
            if (rate && (rate <= 40))
            {
                // Just stay in low-power accel mode.
                lp_accel_mode(rate);
                return 0;
            }
            // Requested rate exceeds the allowed frequencies in LP accel mode,
            // switch back to full-power mode.
            lp_accel_mode(0);
        }
        if (rate < 4)
            rate = 4;
        else if (rate > 1000)
            rate = 1000;

        newRate = 1000 / rate - 1;

        if(!Write(st.reg->rate_div, newRate))
            return -1;

        st.chip_cfg.sample_rate = 1000 / (1 + newRate);
        
        if(_model == MPU9150 || _model == MPU9250)
            set_compass_sample_rate(min(st.chip_cfg.compass_sample_rate, MAX_COMPASS_SAMPLE_RATE));

        // Automatically set LPF to 1/2 sampling rate.
        set_lpf(st.chip_cfg.sample_rate >> 1);
        return 0;
    }
}

//  @brief      Enter low-power accel-only mode.
//  In low-power accel mode, the chip goes to sleep and only wakes up to sample
//  the accelerometer at one of the following frequencies:
//  \n MPU6050: 1.25Hz, 5Hz, 20Hz, 40Hz
//  \n If the requested rate is not one listed above, the device will be set to
//  the next highest rate. Requesting a rate above the maximum supported
//  frequency will result in an error.
//  \n To select a fractional wake-up frequency, round down the value passed to
//  @e rate.
//  @param[in]  rate        Minimum sampling rate, or zero to disable LP
//                          accel mode.
//  @return     0 if successful.
int MPUSeries::lp_accel_mode(unsigned char rate)
{
    unsigned char data[2];

    if (rate > 40)
        return -1;

    if (!rate)
    {
        set_int_latched(0);
        
        data[0] = 0;
        data[1] = BIT_STBY_XYZG;
        if(!Write(st.reg->pwr_mgmt_1, data, 2))
            return -1;
        
        st.chip_cfg.lp_accel_mode = 0;
        return 0;
    }
    // For LP accel, we automatically configure the hardware to produce latched
    // interrupts. In LP accel mode, the hardware cycles into sleep mode before
    // it gets a chance to deassert the interrupt pin; therefore, we shift this
    // responsibility over to the MCU.
    //
    // Any register read will clear the interrupt.
    set_int_latched(1);
    
    if(_model == MPU6050 || _model == MPU9150)
    {
        data[0] = BIT_LPA_CYCLE;
        if (rate == 1)
        {
            data[1] = INV_LPA_1_25HZ;
            set_lpf(5);
        }
        else if (rate <= 5)
        {
            data[1] = INV_LPA_5HZ;
            set_lpf(5);
        }
        else if (rate <= 20)
        {
            data[1] = INV_LPA_20HZ;
            set_lpf(10);
        }
        else
        {
            data[1] = INV_LPA_40HZ;
            set_lpf(20);
        }
        data[1] = (data[1] << 6) | BIT_STBY_XYZG;
        if (!Write(st.reg->pwr_mgmt_1, data, 2))
            return -1;
    }
    else if(_model == MPU6500 || _model == MPU9250)
    {
        // Set wake frequency.
        if (rate == 1)
            data[0] = INV_LPA2_1_25HZ;
        else if (rate == 2)
            data[0] = INV_LPA2_2_5HZ;
        else if (rate <= 5)
            data[0] = INV_LPA2_5HZ;
        else if (rate <= 10)
            data[0] = INV_LPA2_10HZ;
        else if (rate <= 20)
            data[0] = INV_LPA2_20HZ;
        else if (rate <= 40)
            data[0] = INV_LPA2_40HZ;
        else if (rate <= 80)
            data[0] = INV_LPA2_80HZ;
        else if (rate <= 160)
            data[0] = INV_LPA2_160HZ;
        else if (rate <= 320)
            data[0] = INV_LPA2_320HZ;
        else
            data[0] = INV_LPA2_640HZ;
        if (!Write(st.reg->lp_accel_odr, data[0]))
            return -1;
        data[0] = BIT_LPA_CYCLE;
        if (!Write(st.reg->pwr_mgmt_1, data[0]))
            return -1;
    }
    else
        return -1;

    st.chip_cfg.sensors = INV_XYZ_ACCEL;
    st.chip_cfg.clk_src = 0;
    st.chip_cfg.lp_accel_mode = 1;
    
    configure_fifo(0);

    return 0;
}

//  @brief      Enable latched interrupts.
//  Any MPU register will clear the interrupt.
//  @param[in]  enable  1 to enable, 0 to disable.
//  @return     0 if successful.
int MPUSeries::set_int_latched(unsigned char enable)
{
    unsigned char tmp;
    if (st.chip_cfg.latched_int == enable)
        return 0;

    if (enable)
        tmp = BIT_LATCH_EN | BIT_ANY_RD_CLR;
    else
        tmp = 0;
    if (st.chip_cfg.bypass_mode)
        tmp |= BIT_BYPASS_EN;
    if (st.chip_cfg.active_low_int)
        tmp |= BIT_ACTL;

    if(!Write(st.reg->int_pin_cfg, tmp))
        return -1;
    
    st.chip_cfg.latched_int = enable;
    return 0;
}

//  @brief      Select which sensors are pushed to FIFO.
//  @e sensors can contain a combination of the following flags:
//  \n INV_X_GYRO, INV_Y_GYRO, INV_Z_GYRO
//  \n INV_XYZ_GYRO
//  \n INV_XYZ_ACCEL
//  @param[in]  sensors Mask of sensors to push to FIFO.
//  @return     0 if successful.
int MPUSeries::configure_fifo(unsigned char sensors)
{
    unsigned char prev;
    int result = 0;

    // Compass data isn't going into the FIFO. Stop trying.
    sensors &= ~INV_XYZ_COMPASS;

    if (st.chip_cfg.dmp_on)
        return 0;
    else
    {
        if (!(st.chip_cfg.sensors))
            return -1;
        prev = st.chip_cfg.fifo_enable;
        st.chip_cfg.fifo_enable = sensors & st.chip_cfg.sensors;
        if (st.chip_cfg.fifo_enable != sensors)
            // You're not getting what you asked for. Some sensors are asleep.
            result = -1;
        else
            result = 0;
        if (sensors || st.chip_cfg.lp_accel_mode)
            set_int_enable(1);
        else
            set_int_enable(0);
        if (sensors)
        {
            if (reset_fifo())
            {
                st.chip_cfg.fifo_enable = prev;
                return -1;
            }
        }
    }

    return result;
}

//  @brief      Enable/disable data ready interrupt.
//  If the DMP is on, the DMP interrupt is enabled. Otherwise, the data ready
//  interrupt is used.
//  @param[in]  enable      1 to enable interrupt.
//  @return     0 if successful.
int MPUSeries::set_int_enable(unsigned char enable)
{
    unsigned char tmp;

    if (st.chip_cfg.dmp_on)
    {
        if (enable)
            tmp = BIT_DMP_INT_EN;
        else
            tmp = 0x00;        

        if(!Write(st.reg->int_enable, tmp))
            return -1;
        
        st.chip_cfg.int_enable = tmp;
    }
    else
    {
        if (!st.chip_cfg.sensors)
            return -1;
        if (enable && st.chip_cfg.int_enable)
            return 0;
        if (enable)
            tmp = BIT_DATA_RDY_EN;
        else
            tmp = 0x00;
        
        if(!Write(st.reg->int_enable, tmp))
            return -1;
        
        st.chip_cfg.int_enable = tmp;
    }
    return 0;
}

//  @brief  Reset FIFO read/write pointers.
//  @return 0 if successful.
int MPUSeries::reset_fifo()
{
    uint8_t data;
    
    if (!(st.chip_cfg.sensors))
        return -1;

    if(!Write(st.reg->int_enable, 0))
        return -1;
    
    if(!Write(st.reg->fifo_en, 0))
        return -1;

    if(!Write(st.reg->user_ctrl, 0))
        return -1;

    if (st.chip_cfg.dmp_on)
    {
        if(!Write(st.reg->user_ctrl, BIT_FIFO_RST | BIT_DMP_RST))
            return -1;
        
        delay(50);
        
        data = BIT_DMP_EN | BIT_FIFO_EN;
        if (st.chip_cfg.sensors & INV_XYZ_COMPASS)
            data |= BIT_AUX_IF_EN;
        if(!Write(st.reg->user_ctrl, data))
            return -1;

        if (st.chip_cfg.int_enable)
            data = BIT_DMP_INT_EN;
        else
            data = 0;
        if(!Write(st.reg->int_enable, data))
            return -1;
        
        if(!Write(st.reg->fifo_en, 0))
            return -1;
    }
    else
    {
        if(!Write(st.reg->user_ctrl, BIT_FIFO_RST))
            return -1;
        
        if (st.chip_cfg.bypass_mode || !(st.chip_cfg.sensors & INV_XYZ_COMPASS))
            data = BIT_FIFO_EN;
        else
            data = BIT_FIFO_EN | BIT_AUX_IF_EN;
        if(!Write(st.reg->user_ctrl, data))
            return -1;
        
        delay(50);
        
        if (st.chip_cfg.int_enable)
            data = BIT_DATA_RDY_EN;
        else
            data = 0;
        if(!Write(st.reg->int_enable, data))
            return -1;
        
        if(!Write(st.reg->fifo_en, st.chip_cfg.fifo_enable))
            return -1;
    }
    
    return 0;
}

//  @brief      Turn specific sensors on/off.
//  @e sensors can contain a combination of the following flags:
//  \n INV_X_GYRO, INV_Y_GYRO, INV_Z_GYRO
//  \n INV_XYZ_GYRO
//  \n INV_XYZ_ACCEL
//  \n INV_XYZ_COMPASS
//  @param[in]  sensors    Mask of sensors to wake.
//  @return     0 if successful.
int MPUSeries::set_sensors(unsigned char sensors)
{
    uint8_t data;

    if (sensors & INV_XYZ_GYRO)
        data = INV_CLK_PLL;
    else if (sensors)
        data = 0;
    else
        data = BIT_SLEEP;
    if(!Write(st.reg->pwr_mgmt_1, data))
    {
        st.chip_cfg.sensors = 0;
        return -1;
    }
    st.chip_cfg.clk_src = data & ~BIT_SLEEP;
    
    data = 0;
    if (!(sensors & INV_X_GYRO))
        data |= BIT_STBY_XG;
    if (!(sensors & INV_Y_GYRO))
        data |= BIT_STBY_YG;
    if (!(sensors & INV_Z_GYRO))
        data |= BIT_STBY_ZG;
    if (!(sensors & INV_XYZ_ACCEL))
        data |= BIT_STBY_XYZA;
    if(!Write(st.reg->pwr_mgmt_2, data))
    {
        st.chip_cfg.sensors = 0;
        return -1;
    }

    if (sensors && (sensors != INV_XYZ_ACCEL))
        // Latched interrupts only used in LP accel mode.
        set_int_latched(0);
     
     if(_model == MPU9150 || _model == MPU9250)
     {
        unsigned char user_ctrl;

        if (!Read(st.reg->user_ctrl, &user_ctrl, 1))
            return -1;
        // Handle AKM power management.
        if (sensors & INV_XYZ_COMPASS)
        {
            data = AKM_SINGLE_MEASUREMENT;
            user_ctrl |= BIT_AUX_IF_EN;
        }
        else
        {
            data = AKM_POWER_DOWN;
            user_ctrl &= ~BIT_AUX_IF_EN;
        }
        if (st.chip_cfg.dmp_on)
            user_ctrl |= BIT_DMP_EN;
        else
            user_ctrl &= ~BIT_DMP_EN;
        if (!Write(st.reg->s1_do, data))
            return -1;
        // Enable/disable I2C master mode.
        if (!Write(st.reg->user_ctrl, user_ctrl))
            return -1;
     }

    st.chip_cfg.sensors = sensors;
    st.chip_cfg.lp_accel_mode = 0;
    
    delay(50);
    
    return 0;
}

//  @brief      Load and verify DMP image.
//  @param[in]  length      Length of DMP image.
//  @param[in]  firmware    DMP code.
//  @param[in]  start_addr  Starting address of DMP code memory.
//  @param[in]  sample_rate Fixed sampling rate used when DMP is enabled.
//  @return     0 if successful.
int MPUSeries::load_firmware(unsigned short length, const unsigned char *firmware, unsigned short start_addr, unsigned short sample_rate)
{
    unsigned short ii;
    unsigned short this_write;
    int errCode;
    uint8_t *progBuffer;
    /* Must divide evenly into st->hw->bank_size to avoid bank crossings. */
#define LOAD_CHUNK  (16)
    unsigned char cur[LOAD_CHUNK];

    if (st.chip_cfg.dmp_loaded)
        /* DMP should only be loaded once. */
        return -1;

    if (!firmware)
        return -2;
        
    progBuffer = (uint8_t *)malloc(LOAD_CHUNK);
    for (ii = 0; ii < length; ii += this_write)
    {
        this_write = min(LOAD_CHUNK, length - ii);
        
        for (int progIndex = 0; progIndex < this_write; progIndex++)
#ifdef __SAM3X8E__
            progBuffer[progIndex] = firmware[ii + progIndex];
#else
            progBuffer[progIndex] = pgm_read_byte(firmware + ii + progIndex);
#endif
            
        if ((errCode = write_mem(ii, this_write, progBuffer)))
            return -3;
        
        if (read_mem(ii, this_write, cur))
            return -4;
            
        if (memcmp(progBuffer, cur, this_write))
            return -5;
    }

    /* Set program start address. */
    uint8_t data[2];
    data[0] = start_addr >> 8;
    data[1] = start_addr & 0xFF;
    if(!Write(st.reg->prgm_start_h, data, 2))
        return -6;

    st.chip_cfg.dmp_loaded = 1;
    st.chip_cfg.dmp_sample_rate = sample_rate;

    return 0;
}

//  @brief      Write to the DMP memory.
//  This function prevents I2C writes past the bank boundaries. The DMP memory
//  is only accessible when the chip is awake.
//  @param[in]  mem_addr    Memory location (bank << 8 | start address)
//  @param[in]  length      Number of bytes to write.
//  @param[in]  data        Bytes to write to memory.
//  @return     0 if successful.
int MPUSeries::write_mem(unsigned short mem_addr, unsigned short length, unsigned char *data)
{
    unsigned char tmp[2];

    if (!data)
        return -1;
    if (!st.chip_cfg.sensors)
        return -2;

    tmp[0] = (unsigned char)(mem_addr >> 8);
    tmp[1] = (unsigned char)(mem_addr & 0xFF);
    // Check bank boundaries.
    if (tmp[1] + length > st.hw->bank_size)
        return -3;
    
    if(!Write(st.reg->bank_sel, tmp, 2))
        return -4;
    
    if(!Write(st.reg->mem_r_w, data, length))
        return -5;
    
    return 0;
}

//  @brief      Read from the DMP memory.
//  This function prevents I2C reads past the bank boundaries. The DMP memory
//  is only accessible when the chip is awake.
//  @param[in]  mem_addr    Memory location (bank << 8 | start address)
//  @param[in]  length      Number of bytes to read.
//  @param[out] data        Bytes read from memory.
//  @return     0 if successful.
int MPUSeries::read_mem(unsigned short mem_addr, unsigned short length, unsigned char *data)
{
    unsigned char tmp[2];

    if (!data)
        return -1;
    if (!st.chip_cfg.sensors)
        return -1;

    tmp[0] = (unsigned char)(mem_addr >> 8);
    tmp[1] = (unsigned char)(mem_addr & 0xFF);

    /* Check bank boundaries. */
    if (tmp[1] + length > st.hw->bank_size)
        return -1;
        
    if(!Write(st.reg->bank_sel, tmp, 2))
        return -1;
    
    if(!Read(st.reg->mem_r_w, data, length))
        return -1;
    
    return 0;
}

//  @brief      Set DMP output rate.
//  Only used when DMP is on.
//  @param[in]  rate    Desired fifo rate (Hz).
//  @return     0 if successful.
int MPUSeries::set_fifo_rate(unsigned short rate)
{
    const unsigned char regs_end[12] = {DINAFE, DINAF2, DINAAB,
        0xc4, DINAAA, DINAF1, DINADF, DINADF, 0xBB, 0xAF, DINADF, DINADF};
    unsigned short div;
    unsigned char tmp[8];

    if (rate > DMP_SAMPLE_RATE)
        return -1;
    div = DMP_SAMPLE_RATE / rate - 1;
    tmp[0] = (unsigned char)((div >> 8) & 0xFF);
    tmp[1] = (unsigned char)(div & 0xFF);
    if (write_mem(D_0_22, 2, tmp))
        return -1;
    if (write_mem(CFG_6, 12, (unsigned char*)regs_end))
        return -1;

    dmp.fifo_rate = rate;
    return 0;
}

//  @brief      Enable/disable DMP support.
//  @param[in]  enable  1 to turn on the DMP.
//  @return     0 if successful.
int MPUSeries::set_dmp_state(unsigned char enable)
{
    if (st.chip_cfg.dmp_on == enable)
        return 0;

    if (enable)
    {
        if (!st.chip_cfg.dmp_loaded)
            return -1;
        // Disable data ready interrupt.
        set_int_enable(0);
        // Disable bypass mode.
        set_bypass(0);
        // Keep constant sample rate, FIFO rate controlled by DMP.
        set_sample_rate(st.chip_cfg.dmp_sample_rate);
        // Remove FIFO elements.
        Write(0x23, 0);
        st.chip_cfg.dmp_on = 1;
        // Enable DMP interrupt.
        set_int_enable(1);
        reset_fifo();
    }
    else
    {
        // Disable DMP interrupt.
        set_int_enable(0);
        // Restore FIFO settings.
        Write(0x23, st.chip_cfg.fifo_enable);
        st.chip_cfg.dmp_on = 0;
        reset_fifo();
    }
    return 0;
}

//  @brief      Set device to bypass mode.
//  @param[in]  bypass_on   1 to enable bypass mode.
//  @return     0 if successful.
int MPUSeries::set_bypass(unsigned char bypass_on)
{
    unsigned char tmp;

    if (st.chip_cfg.bypass_mode == bypass_on)
        return 0;

    if (bypass_on)
    {
        if (!Read(st.reg->user_ctrl, &tmp, 1))
            return -1;
        tmp &= ~BIT_AUX_IF_EN;
        if (!Write(st.reg->user_ctrl, tmp))
            return -1;
        delay(3);
        tmp = BIT_BYPASS_EN;
        if (st.chip_cfg.active_low_int)
            tmp |= BIT_ACTL;
        if (st.chip_cfg.latched_int)
            tmp |= BIT_LATCH_EN | BIT_ANY_RD_CLR;
        if (!Write(st.reg->int_pin_cfg, tmp))
            return -1;
    }
    else
    {
        // Enable I2C master mode if compass is being used.
        if (!Read(st.reg->user_ctrl, &tmp, 1))
            return -1;
        if (st.chip_cfg.sensors & INV_XYZ_COMPASS)
            tmp |= BIT_AUX_IF_EN;
        else
            tmp &= ~BIT_AUX_IF_EN;
        if (!Write(st.reg->user_ctrl, tmp))
            return -1;
        delay(3);
        if (st.chip_cfg.active_low_int)
            tmp = BIT_ACTL;
        else
            tmp = 0;
        if (st.chip_cfg.latched_int)
            tmp |= BIT_LATCH_EN | BIT_ANY_RD_CLR;
        if (!Write(st.reg->int_pin_cfg, tmp))
            return -1;
    }
    st.chip_cfg.bypass_mode = bypass_on;
    return 0;
}

//  @brief      Push gyro and accel orientation to the dmp->
//  The orientation is represented here as the output of
//  @e orientation_matrix_to_scalar.
//  @param[in]  orient  Gyro and accel orientation in body frame.
//  @return     0 if successful.
int MPUSeries::set_orientation(unsigned short orient)
{
    unsigned char gyro_regs[3], accel_regs[3];
    const unsigned char gyro_axes[3] = {DINA4C, DINACD, DINA6C};
    const unsigned char accel_axes[3] = {DINA0C, DINAC9, DINA2C};
    const unsigned char gyro_sign[3] = {DINA36, DINA56, DINA76};
    const unsigned char accel_sign[3] = {DINA26, DINA46, DINA66};

    gyro_regs[0] = gyro_axes[orient & 3];
    gyro_regs[1] = gyro_axes[(orient >> 3) & 3];
    gyro_regs[2] = gyro_axes[(orient >> 6) & 3];
    accel_regs[0] = accel_axes[orient & 3];
    accel_regs[1] = accel_axes[(orient >> 3) & 3];
    accel_regs[2] = accel_axes[(orient >> 6) & 3];

    // Chip-to-body, axes only.
    if (write_mem(FCFG_1, 3, gyro_regs))
        return -1;
    if (write_mem(FCFG_2, 3, accel_regs))
        return -1;

    memcpy(gyro_regs, gyro_sign, 3);
    memcpy(accel_regs, accel_sign, 3);
    if (orient & 4)
    {
        gyro_regs[0] |= 1;
        accel_regs[0] |= 1;
    }
    if (orient & 0x20)
    {
        gyro_regs[1] |= 1;
        accel_regs[1] |= 1;
    }
    if (orient & 0x100)
    {
        gyro_regs[2] |= 1;
        accel_regs[2] |= 1;
    }

    // Chip-to-body, sign only.
    if (write_mem(FCFG_3, 3, gyro_regs))
        return -1;
    if (write_mem(FCFG_7, 3, accel_regs))
        return -1;
    dmp.orient = orient;
    return 0;
}

// The sensors can be mounted onto the board in any orientation. The mounting
// matrix seen below tells the MPL how to rotate the raw data from thei
// driver(s).
// TODO: The following matrices refer to the configuration on an internal test
// board at Invensense. If needed, please modify the matrices to match the
// chip-to-body matrix for your particular set up.
unsigned short MPUSeries::orientation_matrix_to_scalar(signed char *mtx)
{
    unsigned short scalar;
    /*
       XYZ  010_001_000 Identity Matrix
       XZY  001_010_000
       YXZ  010_000_001
       YZX  000_010_001
       ZXY  001_000_010
       ZYX  000_001_010
     */
    scalar = row_2_scale(mtx);
    scalar |= row_2_scale(mtx + 3) << 3;
    scalar |= row_2_scale(mtx + 6) << 6;
    return scalar;
}

// These next two functions converts the orientation matrix (see
// gyro_orientation) to a scalar representation for use by the DMP.
// NOTE: These functions are borrowed from InvenSense's MPL.
unsigned short MPUSeries::row_2_scale(signed char *row)
{
    unsigned short b;

    if (row[0] > 0)
        b = 0;
    else if (row[0] < 0)
        b = 4;
    else if (row[1] > 0)
        b = 1;
    else if (row[1] < 0)
        b = 5;
    else if (row[2] > 0)
        b = 2;
    else if (row[2] < 0)
        b = 6;
    else
        b = 7;      // error
    return b;
}

//  @brief      Enable DMP features.
//  The following \#define's are used in the input mask:
//  \n DMP_FEATURE_TAP
//  \n DMP_FEATURE_ANDROID_ORIENT
//  \n DMP_FEATURE_LP_QUAT
//  \n DMP_FEATURE_6X_LP_QUAT
//  \n DMP_FEATURE_GYRO_CAL
//  \n DMP_FEATURE_SEND_RAW_ACCEL
//  \n DMP_FEATURE_SEND_RAW_GYRO
//  \n NOTE: DMP_FEATURE_LP_QUAT and DMP_FEATURE_6X_LP_QUAT are mutually
//  exclusive.
//  \n NOTE: DMP_FEATURE_SEND_RAW_GYRO and DMP_FEATURE_SEND_CAL_GYRO are also
//  mutually exclusive.
//  @param[in]  mask    Mask of features to enable.
//  @return     0 if successful.
int MPUSeries::enable_feature(unsigned short mask)
{
    unsigned char tmp[10];

    // TODO: All of these settings can probably be integrated into the default
    // DMP image.
    // Set integration scale factor.
    tmp[0] = (unsigned char)((GYRO_SF >> 24) & 0xFF);
    tmp[1] = (unsigned char)((GYRO_SF >> 16) & 0xFF);
    tmp[2] = (unsigned char)((GYRO_SF >> 8) & 0xFF);
    tmp[3] = (unsigned char)(GYRO_SF & 0xFF);
    write_mem(D_0_104, 4, tmp);

    // Send sensor data to the FIFO.
    tmp[0] = 0xA3;
    if (mask & DMP_FEATURE_SEND_RAW_ACCEL)
    {
        tmp[1] = 0xC0;
        tmp[2] = 0xC8;
        tmp[3] = 0xC2;
    }
    else
    {
        tmp[1] = 0xA3;
        tmp[2] = 0xA3;
        tmp[3] = 0xA3;
    }
    if (mask & DMP_FEATURE_SEND_ANY_GYRO)
    {
        tmp[4] = 0xC4;
        tmp[5] = 0xCC;
        tmp[6] = 0xC6;
    }
    else
    {
        tmp[4] = 0xA3;
        tmp[5] = 0xA3;
        tmp[6] = 0xA3;
    }
    tmp[7] = 0xA3;
    tmp[8] = 0xA3;
    tmp[9] = 0xA3;
    write_mem(CFG_15,10,tmp);

    // Send gesture data to the FIFO.
    if (mask & (DMP_FEATURE_TAP | DMP_FEATURE_ANDROID_ORIENT))
        tmp[0] = DINA20;
    else
        tmp[0] = 0xD8;
    write_mem(CFG_27,1,tmp);

    if (mask & DMP_FEATURE_GYRO_CAL)
        enable_gyro_cal(1);
    else
        enable_gyro_cal(0);

    if (mask & DMP_FEATURE_SEND_ANY_GYRO)
    {
        if (mask & DMP_FEATURE_SEND_CAL_GYRO)
        {
            tmp[0] = 0xB2;
            tmp[1] = 0x8B;
            tmp[2] = 0xB6;
            tmp[3] = 0x9B;
        }
        else
        {
            tmp[0] = DINAC0;
            tmp[1] = DINA80;
            tmp[2] = DINAC2;
            tmp[3] = DINA90;
        }
        write_mem(CFG_GYRO_RAW_DATA, 4, tmp);
    }

    if (mask & DMP_FEATURE_LP_QUAT)
        enable_lp_quat(1);
    else
        enable_lp_quat(0);

    if (mask & DMP_FEATURE_6X_LP_QUAT)
        enable_6x_lp_quat(1);
    else
        enable_6x_lp_quat(0);

    // Pedometer is always enabled.
    dmp.feature_mask = mask | DMP_FEATURE_PEDOMETER;
    reset_fifo();

    dmp.packet_length = 0;
    if (mask & DMP_FEATURE_SEND_RAW_ACCEL)
        dmp.packet_length += 6;
    if (mask & DMP_FEATURE_SEND_ANY_GYRO)
        dmp.packet_length += 6;
    if (mask & (DMP_FEATURE_LP_QUAT | DMP_FEATURE_6X_LP_QUAT))
        dmp.packet_length += 16;
    if (mask & (DMP_FEATURE_TAP | DMP_FEATURE_ANDROID_ORIENT))
        dmp.packet_length += 4;

    return 0;
}

//  @brief      Calibrate the gyro data in the dmp->
//  After eight seconds of no motion, the DMP will compute gyro biases and
//  subtract them from the quaternion output. If @e dmp_enable_feature is
//  called with @e DMP_FEATURE_SEND_CAL_GYRO, the biases will also be
//  subtracted from the gyro output.
//  @param[in]  enable  1 to enable gyro calibration.
//  @return     0 if successful.
int MPUSeries::enable_gyro_cal(unsigned char enable)
{
    if (enable)
    {
        unsigned char regs[9] = {0xb8, 0xaa, 0xb3, 0x8d, 0xb4, 0x98, 0x0d, 0x35, 0x5d};
        return write_mem(CFG_MOTION_BIAS, 9, regs);
    }
    else
    {
        unsigned char regs[9] = {0xb8, 0xaa, 0xaa, 0xaa, 0xb0, 0x88, 0xc3, 0xc5, 0xc7};
        return write_mem(CFG_MOTION_BIAS, 9, regs);
    }
}

//  @brief      Generate 3-axis quaternions from the dmp->
//  In this driver, the 3-axis and 6-axis DMP quaternion features are mutually
//  exclusive.
//  @param[in]  enable  1 to enable 3-axis quaternion.
//  @return     0 if successful.
int MPUSeries::enable_lp_quat(unsigned char enable)
{
    unsigned char regs[4];
    if (enable)
    {
        regs[0] = DINBC0;
        regs[1] = DINBC2;
        regs[2] = DINBC4;
        regs[3] = DINBC6;
    }
    else
        memset(regs, 0x8B, 4);

    write_mem(CFG_LP_QUAT, 4, regs);

    return reset_fifo();
}

//  @brief       Generate 6-axis quaternions from the dmp->
//  In this driver, the 3-axis and 6-axis DMP quaternion features are mutually
//  exclusive.
//  @param[in]   enable  1 to enable 6-axis quaternion.
//  @return      0 if successful.
int MPUSeries::enable_6x_lp_quat(unsigned char enable)
{
    unsigned char regs[4];
    if (enable)
    {
        regs[0] = DINA20;
        regs[1] = DINA28;
        regs[2] = DINA30;
        regs[3] = DINA38;
    }
    else
        memset(regs, 0xA3, 4);

    write_mem(CFG_8, 4, regs);

    return reset_fifo();
}

//  @brief      Get one packet from the FIFO.
//  If @e sensors does not contain a particular sensor, disregard the data
//  returned to that pointer.
//  \n @e sensors can contain a combination of the following flags:
//  \n INV_X_GYRO, INV_Y_GYRO, INV_Z_GYRO
//  \n INV_XYZ_GYRO
//  \n INV_XYZ_ACCEL
//  \n INV_WXYZ_QUAT
//  \n If the FIFO has no new data, @e sensors will be zero.
//  \n If the FIFO is disabled, @e sensors will be zero and this function will
//  return a non-zero error code.
//  @param[out] gyro        Gyro data in hardware units.
//  @param[out] accel       Accel data in hardware units.
//  @param[out] quat        3-axis quaternion data in hardware units.
//  @param[out] timestamp   Timestamp in milliseconds.
//  @param[out] sensors     Mask of sensors read from FIFO.
//  @param[out] more        Number of remaining packets.
//  @return     0 if successful.
int MPUSeries::read_fifo(short *gyro, short *accel, long *quat, unsigned long *timestamp, short *sensors, unsigned char *more)
{
    unsigned char fifo_data[MAX_PACKET_LENGTH];
    unsigned char ii = 0;
    int errCode;

    // TODO: sensors[0] only changes when dmp_enable_feature is called. We can
    // cache this value and save some cycles.
    sensors[0] = 0;

    // Get a packet.
    if ((errCode = read_fifo_stream(dmp.packet_length, fifo_data, more)))
        return errCode;

    // Parse DMP packet.
    if (dmp.feature_mask & (DMP_FEATURE_LP_QUAT | DMP_FEATURE_6X_LP_QUAT))
    {
#ifdef FIFO_CORRUPTION_CHECK
        long quat_q14[4], quat_mag_sq;
#endif
        quat[0] = ((long)fifo_data[0] << 24) | ((long)fifo_data[1] << 16) |
            ((long)fifo_data[2] << 8) | fifo_data[3];
        quat[1] = ((long)fifo_data[4] << 24) | ((long)fifo_data[5] << 16) |
            ((long)fifo_data[6] << 8) | fifo_data[7];
        quat[2] = ((long)fifo_data[8] << 24) | ((long)fifo_data[9] << 16) |
            ((long)fifo_data[10] << 8) | fifo_data[11];
        quat[3] = ((long)fifo_data[12] << 24) | ((long)fifo_data[13] << 16) |
            ((long)fifo_data[14] << 8) | fifo_data[15];
        ii += 16;
#ifdef FIFO_CORRUPTION_CHECK
        // We can detect a corrupted FIFO by monitoring the quaternion data and
        // ensuring that the magnitude is always normalized to one. This
        // shouldn't happen in normal operation, but if an I2C error occurs,
        // the FIFO reads might become misaligned.
        //
        // Let's start by scaling down the quaternion data to avoid long long
        // math.
        quat_q14[0] = quat[0] >> 16;
        quat_q14[1] = quat[1] >> 16;
        quat_q14[2] = quat[2] >> 16;
        quat_q14[3] = quat[3] >> 16;
        quat_mag_sq = quat_q14[0] * quat_q14[0] + quat_q14[1] * quat_q14[1] +
            quat_q14[2] * quat_q14[2] + quat_q14[3] * quat_q14[3];
        if ((quat_mag_sq < QUAT_MAG_SQ_MIN) || (quat_mag_sq > QUAT_MAG_SQ_MAX))
        {
            // Quaternion is outside of the acceptable threshold.
            reset_fifo();
            sensors[0] = 0;
            return 3;
        }
        sensors[0] |= INV_WXYZ_QUAT;
#endif
    }

    if (dmp.feature_mask & DMP_FEATURE_SEND_RAW_ACCEL)
    {
        accel[0] = ((short)fifo_data[ii+0] << 8) | fifo_data[ii+1];
        accel[1] = ((short)fifo_data[ii+2] << 8) | fifo_data[ii+3];
        accel[2] = ((short)fifo_data[ii+4] << 8) | fifo_data[ii+5];
        ii += 6;
        sensors[0] |= INV_XYZ_ACCEL;
    }

    if (dmp.feature_mask & DMP_FEATURE_SEND_ANY_GYRO)
    {
        gyro[0] = ((short)fifo_data[ii+0] << 8) | fifo_data[ii+1];
        gyro[1] = ((short)fifo_data[ii+2] << 8) | fifo_data[ii+3];
        gyro[2] = ((short)fifo_data[ii+4] << 8) | fifo_data[ii+5];
        ii += 6;
        sensors[0] |= INV_XYZ_GYRO;
    }

    if (timestamp)
	    *timestamp = millis();
    
    return 0;
}

//  @brief      Get one unparsed packet from the FIFO.
//  This function should be used if the packet is to be parsed elsewhere.
//  @param[in]  length  Length of one FIFO packet.
//  @param[in]  data    FIFO packet.
//  @param[in]  more    Number of remaining packets.
int MPUSeries::read_fifo_stream(unsigned short length, unsigned char *data, unsigned char *more)
{
    unsigned char tmp[2];
    unsigned short fifo_count;
    if (!st.chip_cfg.dmp_on)
        return -1;
    if (!st.chip_cfg.sensors)
        return -2;

    if (!Read(st.reg->fifo_count_h, tmp, 2))
        return -3;
    fifo_count = (tmp[0] << 8) | tmp[1];
    if (fifo_count < length)
    {
        more[0] = 0;
        return 1;
    }
    if (fifo_count > (st.hw->max_fifo >> 1))
    {
        // FIFO is 50% full, better check overflow bit.
        if (!Read(st.reg->int_status, tmp, 1))
            return -5;
        if (tmp[0] & BIT_FIFO_OVERFLOW)
        {
            reset_fifo();
            return 2;
        }
    }

    if (!Read(st.reg->fifo_r_w, data, length))
        return -7;
    more[0] = fifo_count / length - 1;
    return 0;
}

/*
// This initialization is similar to the one in ak8975.c
int MPUSeries::setup_compass(void)
{
    if(_model != MPU9150 && _model != MPU9250)
        return -16;
        
    unsigned char data[4], akm_addr;

    set_bypass(1);

    // Find compass. Possible addresses range from 0x0C to 0x0F.
    for (akm_addr = 0x0C; akm_addr <= 0x0F; akm_addr++)
    {
        if(!Read(akm_addr, AKM_REG_WHOAMI, data, 1))
            break;
        if (data[0] == AKM_WHOAMI)
            break;
    }

    if (akm_addr > 0x0F)
    {
        // TODO: Handle this case in all compass-related functions.
        return -1;
    }

    st.chip_cfg.compass_addr = akm_addr;

    data[0] = AKM_POWER_DOWN;
    if (!Write(st.chip_cfg.compass_addr, AKM_REG_CNTL, data, 1))
        return -2;
    delay(1);

    data[0] = AKM_FUSE_ROM_ACCESS;
    if (!Write(st.chip_cfg.compass_addr, AKM_REG_CNTL, data, 1))
        return -3;
    delay(1);

    // Get sensitivity adjustment data from fuse ROM.
    if (!Read(st.chip_cfg.compass_addr, AKM_REG_ASAX, data, 3))
        return -4;
    st.chip_cfg.mag_sens_adj[0] = (long)data[0] + 128;
    st.chip_cfg.mag_sens_adj[1] = (long)data[1] + 128;
    st.chip_cfg.mag_sens_adj[2] = (long)data[2] + 128;

    data[0] = AKM_POWER_DOWN;
    if (!Write(st.chip_cfg.compass_addr, AKM_REG_CNTL, data, 1))
        return -5;
    delay(1);

    set_bypass(0);

    // Set up master mode, master clock, and ES bit.
    data[0] = 0x40;
    if (!Write(st.reg->i2c_mst, data[0]))
        return -6;

    // Slave 0 reads from AKM data registers.
    data[0] = BIT_I2C_READ | st.chip_cfg.compass_addr;
    if (!Write(st.reg->s0_addr, data[0]))
        return -7;

    // Compass reads start at this register.
    data[0] = AKM_REG_ST1;
    if (!Write(st.reg->s0_reg, data[0]))
        return -8;

    // Enable slave 0, 8-byte reads.
    data[0] = BIT_SLAVE_EN | 8;
    if (!Write(st.reg->s0_ctrl, data[0]))
        return -9;

    // Slave 1 changes AKM measurement mode.
    data[0] = st.chip_cfg.compass_addr;
    if (!Write(st.reg->s1_addr, data[0]))
        return -10;

    // AKM measurement mode register.
    data[0] = AKM_REG_CNTL;
    if (!Write(st.reg->s1_reg, data[0]))
        return -11;

    // Enable slave 1, 1-byte writes.
    data[0] = BIT_SLAVE_EN | 1;
    if (!Write(st.reg->s1_ctrl, data[0]))
        return -12;

    // Set slave 1 data.
    data[0] = AKM_SINGLE_MEASUREMENT;
    if (!Write(st.reg->s1_do, data[0]))
        return -13;

    // Trigger slave 0 and slave 1 actions at each sample.
    data[0] = 0x03;
    if (!Write(st.reg->i2c_delay_ctrl, data[0]))
        return -14;

    if(_model == MPU9150)
    {
        // For the MPU9150, the auxiliary I2C bus needs to be set to VDD.
        data[0] = BIT_I2C_MST_VDDIO;
        if (!Write(st.reg->yg_offs_tc, data[0]))
            return -15;
    }

    return 0;
}
*/

//  @brief      Set compass sampling rate.
//  The compass on the auxiliary I2C bus is read by the MPU hardware at a
//  maximum of 100Hz. The actual rate can be set to a fraction of the gyro
//  sampling rate.
//
//  \n WARNING: The new rate may be different than what was requested. Call
//  mpu_get_compass_sample_rate to check the actual setting.
//  @param[in]  rate    Desired compass sampling rate (Hz).
//  @return     0 if successful.
int MPUSeries::set_compass_sample_rate(unsigned short rate)
{
    if(_model != MPU9150 && _model != MPU9250)
        return -1;
    
    unsigned char div;
    if (!rate || rate > st.chip_cfg.sample_rate || rate > MAX_COMPASS_SAMPLE_RATE)
        return -1;

    div = st.chip_cfg.sample_rate / rate - 1;
    if (!Write(st.reg->s4_ctrl, div))
        return -1;
    
    st.chip_cfg.compass_sample_rate = st.chip_cfg.sample_rate / (div + 1);
    return 0;
}