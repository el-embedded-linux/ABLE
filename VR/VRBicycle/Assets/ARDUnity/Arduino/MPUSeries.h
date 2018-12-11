/*
  MPUSeries.h - Ardunity Arduino library
  Copyright (C) 2016 ojh6t3k.  All rights reserved.
*/
#ifndef MPUSeries_h
#define MPUSeries_h

#include "ArdunityI2C.h"

#define MPU6050		0
#define MPU6500		1
#define MPU9150		2
#define MPU9250		3

// Hardware registers needed by driver.
struct gyro_reg_s
{
    unsigned char who_am_i;
    unsigned char rate_div;
    unsigned char lpf;
    unsigned char prod_id;
    unsigned char user_ctrl;
    unsigned char fifo_en;
    unsigned char gyro_cfg;
    unsigned char accel_cfg;
    unsigned char accel_cfg2;
    unsigned char lp_accel_odr;
    unsigned char motion_thr;
    unsigned char motion_dur;
    unsigned char fifo_count_h;
    unsigned char fifo_r_w;
    unsigned char raw_gyro;
    unsigned char raw_accel;
    unsigned char temp;
    unsigned char int_enable;
    unsigned char dmp_int_status;
    unsigned char int_status;
    unsigned char accel_intel;
    unsigned char pwr_mgmt_1;
    unsigned char pwr_mgmt_2;
    unsigned char int_pin_cfg;
    unsigned char mem_r_w;
    unsigned char accel_offs;
    unsigned char i2c_mst;
    unsigned char bank_sel;
    unsigned char mem_start_addr;
    unsigned char prgm_start_h;
    // AK89xx_SECONDARY
    unsigned char s0_addr;
    unsigned char s0_reg;
    unsigned char s0_ctrl;
    unsigned char s1_addr;
    unsigned char s1_reg;
    unsigned char s1_ctrl;
    unsigned char s4_ctrl;
    unsigned char s0_do;
    unsigned char s1_do;
    unsigned char i2c_delay_ctrl;
    unsigned char raw_compass;
    // The I2C_MST_VDDIO bit is in this register.
    unsigned char yg_offs_tc;
};

// Information specific to a particular device.
struct hw_s
{
    unsigned char addr;
    unsigned short max_fifo;
    unsigned char num_reg;
    unsigned short temp_sens;
    short temp_offset;
    unsigned short bank_size;
    // AK89xx_SECONDARY
    unsigned short compass_fsr;
};

struct motion_int_cache_s
{
    unsigned short gyro_fsr;
    unsigned char accel_fsr;
    unsigned short lpf;
    unsigned short sample_rate;
    unsigned char sensors_on;
    unsigned char fifo_sensors;
    unsigned char dmp_on;
};

// Cached chip configuration data.
// TODO: A lot of these can be handled with a bitmask.
struct chip_cfg_s
{
    // Matches gyro_cfg >> 3 & 0x03
    unsigned char gyro_fsr;
    // Matches accel_cfg >> 3 & 0x03
    unsigned char accel_fsr;
    // Enabled sensors. Uses same masks as fifo_en, NOT pwr_mgmt_2.
    unsigned char sensors;
    // Matches config register.
    unsigned char lpf;
    unsigned char clk_src;
    // Sample rate, NOT rate divider.
    unsigned short sample_rate;
    // Matches fifo_en register.
    unsigned char fifo_enable;
    // Matches int enable register.
    unsigned char int_enable;
    // 1 if devices on auxiliary I2C bus appear on the primary.
    unsigned char bypass_mode;
    // 1 if half-sensitivity.
    // NOTE: This doesn't belong here, but everything else in hw_s is const,
    // and this allows us to save some precious RAM.
    unsigned char accel_half;
    // 1 if device in low-power accel-only mode.
    unsigned char lp_accel_mode;
    // 1 if interrupts are only triggered on motion events.
    unsigned char int_motion_only;
    struct motion_int_cache_s cache;
    // 1 for active low interrupts.
    unsigned char active_low_int;
    // 1 for latched interrupts.
    unsigned char latched_int;
    // 1 if DMP is enabled.
    unsigned char dmp_on;
    // Ensures that DMP will only be loaded once.
    unsigned char dmp_loaded;
    // Sampling rate used when DMP is enabled.
    unsigned short dmp_sample_rate;
    // AK89xx_SECONDARY
    // Compass sample rate.
    unsigned short compass_sample_rate;
    unsigned char compass_addr;
    short mag_sens_adj[3];
};

// Information for self-test-> 
struct test_s
{
    unsigned long gyro_sens;
    unsigned long accel_sens;
    unsigned char reg_rate_div;
    unsigned char reg_lpf;
    unsigned char reg_gyro_fsr;
    unsigned char reg_accel_fsr;
    unsigned short wait_ms;
    unsigned char packet_thresh;
    float min_dps;
    float max_dps;
    float max_gyro_var;
    float min_g;
    float max_g;
    float max_accel_var;
};

// Gyro driver state variables.
struct gyro_state_s
{
    struct gyro_reg_s *reg;
    struct hw_s *hw;
    struct chip_cfg_s chip_cfg;
    struct test_s *test;
};

struct dmp_s
{
    void (*tap_cb)(unsigned char count, unsigned char direction);
    void (*android_orient_cb)(unsigned char orientation);
    unsigned short orient;
    unsigned short feature_mask;
    unsigned short fifo_rate;
    unsigned char packet_length;
};

struct s_mympu
{
	float xyzw[4];
	float gyro[3];
};

struct s_quat { float w, x, y, z; }; 
struct s_vec3 { float x, y, z; };

union u_quat
{
    struct s_quat _f;
    long _l[4];
};


class MPUSeries : public ArdunityI2C
{
public:
	MPUSeries(int id, int model, boolean secondary, signed char *orientMatrix);

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnExecute();
	void OnFlush();

private:
    FLOAT32 _qX;
    FLOAT32 _qY;
    FLOAT32 _qZ;
	FLOAT32 _qW;
	boolean _initialized;
    signed char *_orientMatrix;
	int _model;
    struct gyro_reg_s reg;
    struct hw_s hw;
    struct test_s test;
    struct gyro_state_s st;
    struct dmp_s dmp;
    struct s_mympu mympu;

    int ret;
    short gyro[3];
    short sensors;
    unsigned char fifoCount;
    union u_quat q;

	
	boolean initialize();
	boolean update();
	int set_gyro_fsr(unsigned short fsr);
	int set_accel_fsr(unsigned char fsr);
	int set_lpf(unsigned short lpf);
	int set_sample_rate(unsigned short rate);
	int lp_accel_mode(unsigned char rate);
	int set_int_latched(unsigned char enable);
	int configure_fifo(unsigned char sensors);
	int set_int_enable(unsigned char enable);
	int reset_fifo();
	int set_sensors(unsigned char sensors);
	int load_firmware(unsigned short length, const unsigned char *firmware, unsigned short start_addr, unsigned short sample_rate);
	int write_mem(unsigned short mem_addr, unsigned short length, unsigned char *data);
	int read_mem(unsigned short mem_addr, unsigned short length, unsigned char *data);
	int set_fifo_rate(unsigned short rate);
	int set_dmp_state(unsigned char enable);
	int set_bypass(unsigned char bypass_on);
	int set_orientation(unsigned short orient);
	unsigned short orientation_matrix_to_scalar(signed char *mtx);
	unsigned short row_2_scale(signed char *row);
	int enable_feature(unsigned short mask);
	int enable_gyro_cal(unsigned char enable);
	int enable_lp_quat(unsigned char enable);
	int enable_6x_lp_quat(unsigned char enable);
	int read_fifo(short *gyro, short *accel, long *quat, unsigned long *timestamp, short *sensors, unsigned char *more);
	int read_fifo_stream(unsigned short length, unsigned char *data, unsigned char *more);
   // int setup_compass(void);
    int set_compass_sample_rate(unsigned short rate);
};

#endif

